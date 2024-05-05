using System.Text.RegularExpressions;

namespace FfSolver;

public static class BoardHelper
{
    private static readonly Regex cardRegex = new Regex(@"(?'rank'\d{1,2}|J|Q|K)(?'suit'[RGBY]?)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses a board state from the specified cascade and cell string.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="cascadesString">Cascade string</param>
    /// <param name="cellString">Optional cell string</param>
    /// <returns></returns>
    public static Board Parse(string cascadesString, string cellString = "")
    {
        var cardStrings = cascadesString.Split(new[] {' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var cascades = Enumerable.Range(0, Board.CascadeCount).Select(i => new List<Card>()).ToArray();

        var index = 0;
        foreach (var cardString in cardStrings)
        {
            var i = index % Board.CascadeCount;
            var j = index / Board.CascadeCount;

            Card? c;
            try
            {
                c = ParseCard(cardString);
            }
            catch (Exception)
            {
                throw new FormatException($"Could not parse card string '{cardString}' in column {i}, row {j}.");
            }
            
            if (c is Card card)
            {
                if (cascades[i].Count < j)
                {
                    throw new FormatException($"Unexpected card '{cardString}' in column {i}, row {j}.");
                }

                cascades[i].Add(card);
            }

            index++;
        }

        Card? cell;
        try
        {
            cell = string.IsNullOrEmpty(cellString) ? default : ParseCard(cellString);
        }
        catch (Exception)
        {
            throw new FormatException($"Could not parse cell card string '{cellString}'.");
        }

        return new Board(cascades, cell);
    }

    /// <summary>
    /// Parses a single card position from the specified string
    /// </summary>
    /// <remarks>
    /// Values for <paramref name="cardString"/> must not contain leading or trailing whitespace.
    /// A card string may have the following values:
    /// <list type="bullet">
    /// <item><c>-</c> represents an empty position</item>
    /// <item><c>(2|3|4|5|6|7|8|9|10|J|Q|K)(R|G|B|Y)</c> represents a minor arcana card of rank 2-10, Jack, Queen or King and suit of Red, Green, Blue or Yellow (e.g., <c>10G</c>)</item>
    /// <item><c>0-21</c> represents a major arcana card of rank 0-21</item>
    /// </list>
    /// </remarks>
    /// <param name="cardString">String specifying a single card</param>
    /// <returns>Parsed card or null if the position is empty</returns>
    /// <exception cref="ArgumentNullException">Card string is null</exception>
    /// <exception cref="FormatException">Card string is invalid</exception>
    public static Card? ParseCard(string cardString)
    {
        if (cardString is null)
        {
            throw new ArgumentNullException(nameof(cardString));
        }

        if (cardString == "-")
        {
            return default;
        }

        var match = cardRegex.Match(cardString);

        if (!match.Success)
        {
            throw new FormatException("Invalid card");
        }

        var suit = match.Groups["suit"].Value.ToUpper() switch {
            "R" => Suit.Red,
            "G" => Suit.Green,
            "B" => Suit.Blue,
            "Y" => Suit.Yellow,
            "" => Suit.MajorArc,
            _ => throw new FormatException("Invalid suit")
        };

        var rankString = match.Groups["rank"].Value.ToUpper();
        var rank = int.TryParse(rankString, out var r) ? r : -1;

        if (suit != Suit.MajorArc)
        {
            if (rank == -1)
            {
                rank = rankString switch
                {
                    "J" => Card.JackRank,
                    "Q" => Card.QueenRank,
                    "K" => Card.KingRank,
                    _ => throw new FormatException("Invalid rank")
                };
            }
            else if (rank < 2 || rank > 10)
            {
                throw new FormatException("Invalid rank");
            }
        }
        else if (rank < 0 || rank > 21)
        {
            throw new FormatException("Invalid rank");
        }

        return new Card(rank, suit);
    }

    /// <summary>
    /// Returns a pseudo-randomized board using the specified seed.
    /// </summary>
    /// <param name="seed">Seed</param>
    /// <returns>Randomized board</returns>
    public static Board CreateRandomFromSeed(ulong seed)
    {
        var orderValues = Enumerable.Repeat(new Xoshiro256PlusPlus(seed), 70).Select(rng => rng.Next());
        var shuffledDeck = Card.CreateDeck()
            .Zip(orderValues, (card, order) => (card, order))
            .OrderBy(t => t.order)
            .Select(t => t.card);

        var cascades = shuffledDeck.Select((c, i) => (card: c, cascade: i / 7))
            .GroupBy(t => t.cascade)
            .Select(g => g.Select(t => t.card).ToList())
            .ToList();

        cascades.Insert(5, new List<Card>());
        return new Board(cascades);        
    }
}