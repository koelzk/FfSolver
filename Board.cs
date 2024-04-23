using System.Diagnostics;
using System.Text;

namespace FfSolver;

public class Board : IEquatable<Board>
{
    internal const int CascadeCount = 11;

    private Card? cell;
    private readonly List<Card>[] cascades;

    private sbyte arcanaLowFdn = Card.MinorArcMinRank - 1;
    private sbyte arcanaHighFdn = Card.MajorArcMaxRank + 1;
    private byte[] colorFdns = [Card.AceRank, Card.AceRank, Card.AceRank, Card.AceRank];

    public Board(IReadOnlyCollection<IReadOnlyCollection<Card>> cascades, Card? cell = default)
    {
        if (cascades is null)
        {
            throw new ArgumentNullException(nameof(cascades));
        }

        if (cascades.Count != CascadeCount)
        {
            throw new ArgumentException("Invalid number of cascades", nameof(cascades));
        }

        this.cascades = cascades.Select(cc => cc.ToList()).ToArray();
        this.cell = cell;

        UpdateFoundations();
    }

    public Board(Board other)
    {
        cell = other.cell;
        cascades = other.cascades.Select(cc => cc.ToList()).ToArray();
        arcanaLowFdn = other.arcanaLowFdn;
        arcanaHighFdn = other.arcanaHighFdn;
        colorFdns = other.colorFdns.ToArray();
    }

    public bool IsGameWon => arcanaLowFdn == arcanaHighFdn && colorFdns.All(v => v == Card.KingRank);

    public IReadOnlyList<IReadOnlyList<Card>> Cascades => cascades;

    public Card? Cell => cell;

    public void ApplyAutoMoves()
    {
        bool appliedMoves;
        do
        {
            appliedMoves = false;
            foreach (var move in EnumerateAutoMoves())
            {
                ApplyMove(move);
                appliedMoves = true;
            }
        }
        while (appliedMoves);
    }

    public void NormalizeOrder()
    {
        byte GetCascadeValue(List<Card> cascade) => cascade.Count > 0 ? cascade.First().Value : byte.MaxValue;

        var comparer = Comparer<List<Card>>.Create((a,b) => GetCascadeValue(a).CompareTo(GetCascadeValue(b)));
        Array.Sort(cascades, comparer);

        if (arcanaLowFdn == arcanaHighFdn)
        {
            arcanaLowFdn = 21;
            arcanaHighFdn = 21;
        }
    }

    public void Normalize()    
    {
        ApplyAutoMoves();
        NormalizeOrder();
    }

    public IEnumerable<Move> EnumerateAutoMoves()
    {
        for (var i = 0; i < CascadeCount; i++)
        {
            if (cascades[i].Count == 0)
            {
                continue;
            }

            if (CanRemoveCard(cascades[i].Last()))
            {
                yield return new Move(i, Move.Foundation);
            }
        }

        if (cell is Card card && CanRemoveCard(card))
        {
            yield return new Move(Move.Cell, Move.Foundation);
        }
    }

    public IEnumerable<Move> EnumerateMoves()
    {
        // Move k cards from cascade i to cascade j:
        for (var i = 0; i < CascadeCount; i++)
        {
            if (cascades[i].Count == 0)
            {
                continue;
            }

            var stackSize = GetStackSize(cascades[i]);

            for (var j = 0; j < CascadeCount; j++)
            {
                if (i == j)
                {
                    continue;
                }

                for (var k = stackSize; k > 0; k--)
                {
                    if (cascades[j].Count == 0 || cascades[i].Last().CanPlaceOn(cascades[j].Last()))

                    yield return new Move(i, j, k);
                }
            }
        }

        if (cell.HasValue)
        {
            // Move 1 card from cell to cascade j:
            for (var j = 0; j < CascadeCount; j++)
            {
                if (cascades[j].Count == 0 || cell.Value.CanPlaceOn(cascades[j].Last()))
                {
                    yield return new Move(Move.Cell, j);
                }
            }
        }
        else
        {
            // Move 1 card from cascade to cell:
            for (var i = 0; i < CascadeCount; i++)
            {
                if (cascades[i].Count > 0)
                {
                    yield return new Move(i, Move.Cell);
                }
            }
        }
    }

    public void ApplyMove(Move move)
    {
        if (move.From >= 0 && move.To >= 0) // Move from cascade to cascade:
        {
            Debug.Assert(move.From >= 0 && move.From < CascadeCount);
            Debug.Assert(move.To >= 0 && move.To < CascadeCount);

            var from = cascades[move.From];
            var to = cascades[move.To];

            Debug.Assert(from.Count >= move.Count);

            for (var k = 0; k < move.Count; k++)
            {
                to.Add(from.Last());
                from.RemoveAt(from.Count - 1);
            }

            return;
        }        
        else if (move.To == Move.Foundation) // Move from cascade or cell to foundation:
        {
            if (move.From == Move.Cell)
            {
                Debug.Assert(cell.HasValue);
                UpdateFoundation(cell.Value);
                cell = null;
            }
            else
            {
                Debug.Assert(move.From >= 0 && move.From < CascadeCount);
                var from = cascades[move.From];
                Debug.Assert(from.Count > 0);

                UpdateFoundation(from.Last());

                from.RemoveAt(from.Count - 1);
            }

            return;
        }
        else if (move.From == Move.Cell) // Move from cell to cascade:
        {
            Debug.Assert(cell.HasValue);
            Debug.Assert(move.To >= 0 && move.To < CascadeCount);
            cascades[move.To].Add(cell.Value);
            cell = null;

            return;
        }
        else // Move from cascade to cell:
        {            
            Debug.Assert(move.From >= 0 && move.From < CascadeCount);
            Debug.Assert(!cell.HasValue);

            var from = cascades[move.From];
            Debug.Assert(from.Count > 0);

            cell = from.Last();
            from.RemoveAt(from.Count - 1);

            return;
        }
    }

    private void UpdateFoundation(Card removedCard)
    {
        if (removedCard.Suit == Suit.Arcana)
        {
            Debug.Assert(removedCard.Rank == arcanaLowFdn + 1 || removedCard.Rank == arcanaHighFdn - 1);
            if (removedCard.Rank == arcanaLowFdn + 1)
            {
                arcanaLowFdn++;
            }
            
            if (removedCard.Rank == arcanaHighFdn - 1)
            {
                arcanaHighFdn--;
            }
        }
        else
        {
            var suitIndex = (int)removedCard.Suit;
            Debug.Assert(removedCard.Rank == colorFdns[suitIndex] + 1);
            colorFdns[suitIndex]++;
        }
    }

    public int GetScore(int step)
    {
        var score = 0;

        score -= cascades.Sum(cc => cc.Count);
        score += cascades.Select(cc => 
        {
            var stack = GetStackSize(cc);
            if (cc.Count == 0)
            {
                return 20;
            }
            else if (cc.Count == stack)
            {
                return stack * 2;
            }
            else
            {
                return stack;
            }
        }).Sum();
        score -= cell.HasValue ? 10 : 0;
        score -= step;

        return score;
    }

    private int GetStackSize(IReadOnlyList<Card> cascade)
    {
        if (cascade.Count < 2)
        {
            return cascade.Count;
        }

        Card previousCard = cascade.Last();
        var stackSize = 1;
        for (var i = cascade.Count - 2; i >= 0; i--)
        {
            var card = cascade[i];
            if (!previousCard.CanPlaceOn(card))
            {
                break;
            }

            previousCard = card;
            stackSize++;
        }

        return stackSize;
    }

    public static Board CreateRandom(Random random)
    {
        var deck = Card.CreateDeck().ToArray();
        random.Shuffle(deck);

        List<List<Card>> cascades = deck.Select((c, i) => (card: c, cascade: i / 7))
            .GroupBy(t => t.cascade)
            .Select(g => g.Select(t => t.card).ToList())
            .ToList();

        cascades.Insert(5, new List<Card>());
        return new Board(cascades);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        var fdnStrings = Enumerable.Range(0, 4)
            .Select(i => $"{(colorFdns[i] < 2 ? "-" : new Card(colorFdns[i], (Suit)i).ToString()), 4}");

        sb.AppendLine($"{arcanaLowFdn,4} {arcanaHighFdn,4}     {cell?.ToString() ?? "-",4}       {string.Join(" ", fdnStrings)}");

        var maxCount = cascades.Select(cc => cc.Count).Max();
        for (var row = 0; row < maxCount; row++)
        {
            var rowString = cascades.Select(cc => row < cc.Count ? cc[row] : default(Card?))
                .Select(c => $"{c,4}");
            sb.AppendLine(string.Concat(rowString));
        }

        return sb.ToString();
    }

    public bool Equals(Board? other)
    {
        if (other is null)
        {
            return false;
        }

        Debug.Assert(cascades.Length == CascadeCount && other.cascades.Length == CascadeCount);

        if (!cell.Equals(other.cell))
        {
            return false;
        }

        for (var i = 0; i < CascadeCount; i++)
        {
            if (!Enumerable.SequenceEqual(cascades[i], other.cascades[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is Board other && Equals(other);

    private void UpdateFoundations()
    {
        var allCards = cascades.SelectMany(cc => cc).ToList();
        if (cell.HasValue)
        {
            allCards.Add(cell.Value);
        }

        var lowFdns = allCards.GroupBy(c => (int)c.Suit)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Rank).Min() - 1);

        for (var i = 0; i < 4; i++)
        {
            colorFdns[i] = lowFdns.TryGetValue(i, out var f)
                ? (byte)f
                : (byte)Card.KingRank;
        }

        if (lowFdns.TryGetValue(4, out var arcLow))
        {
            arcanaLowFdn =  (sbyte)arcLow;
            arcanaHighFdn = (sbyte)(allCards.Where(c => c.Suit == Suit.Arcana).Select(c => c.Rank).Max() + 1);
        }
        else
        {
            arcanaLowFdn = 21;
            arcanaHighFdn = 21;
        }
    }

    private bool CanRemoveCard(Card card)
    {
        if (card.Suit == Suit.Arcana)
        {
            return (card.Rank == arcanaLowFdn + 1) || (card.Rank == arcanaHighFdn - 1);
        }

        if (cell.HasValue) // Cell must be empty to remove color cards
        {
            return false;
        }

        var fdn = colorFdns[(int)card.Suit];
        return card.Rank == fdn + 1;
    }

    private int GetHashCode(IReadOnlyList<Card> cards)
    {
        int res = 0x2D2816FE;

        foreach (var card in cards)
        {
            res = res * 31 + card.GetHashCode();
        }
        return res;
    }

    public override int GetHashCode()
    {
        var hash = cell.GetHashCode();

        foreach (var cascade in cascades)
        {
            hash = hash * 31 + GetHashCode(cascade);
        }

        return hash;
    }
}
