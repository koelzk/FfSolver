using System.Diagnostics.CodeAnalysis;

namespace FfSolver;

/// <summary>
/// Represents a single card
/// </summary>
public readonly struct Card : IEquatable<Card>
{
    public const int AceRank = 1;
    public const int JackRank = 11;
    public const int QueenRank = 12;
    public const int KingRank = 13;

    public const int MinorArcMinRank = 2;
    public const int MinorArcMaxRank = KingRank;

    public const int MajorArcMinRank = 0;    

    public const int MajorArcMaxRank = 21;

    private const string suitString = "RGBYA";
    private readonly static string[] rankStrings = ["?", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"];

    private readonly byte value;

    public Card(int rank, Suit suit) => 
        value =
            (suit == Suit.MajorArc && (rank < MajorArcMinRank || rank > MajorArcMaxRank)) ||
            (suit != Suit.MajorArc && (rank < MinorArcMinRank || rank > MinorArcMaxRank))
            ? throw new ArgumentException($"Invalid rank {rank}", nameof(rank))
            : (byte)(rank | ((int)suit << 5));

    public int Rank => value & ((1 << 5) - 1);

    public Suit Suit => (Suit)(value >> 5);

    public byte Value => value;

    public bool CanPlaceOn(Card other)
    {
        if (Suit != other.Suit)
        {
            return false;
        }

        return (Rank == other.Rank - 1) || (Rank == other.Rank + 1);
    }

    public override string ToString() => Suit == Suit.MajorArc
        ? GetRankString()
        : GetRankString() + suitString[(int)Suit];

    public string GetRankString() => Suit == Suit.MajorArc ? Rank.ToString() : rankStrings[Rank];

    public static IEnumerable<Card> CreateDeck()
    {
        for (var i = MajorArcMinRank; i <= MajorArcMaxRank; i++)
        {
            yield return new Card(i, Suit.MajorArc);
        }

        foreach (var suit in new[] { Suit.Red, Suit.Green, Suit.Blue, Suit.Yellow })
        {
            for (var i = MinorArcMinRank; i <= MinorArcMaxRank; i++)
            {
                yield return new Card(i, suit);
            }
        }
    }

    public bool Equals(Card other) => value == other.value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Card other && Equals(other);

    public override int GetHashCode() => value;
}
