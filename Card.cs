using System.Diagnostics.CodeAnalysis;

namespace FfSolver;
public readonly struct Card : IEquatable<Card>
{
    private const string suitString = "RGBYA";
    private readonly static string[] rankStrings = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"];

    private readonly byte value;

    public Card(int rank, Suit suit) => 
        value =
            (suit == Suit.Arcana && (rank < 0 || rank > 21)) ||
            (suit != Suit.Arcana && (rank < 2 || rank > 13))
            ? throw new ArgumentException($"Invalid rank {rank}", nameof(rank))
            : (byte)(rank | ((int)suit << 5));

    public int Rank => value & 0x1f;

    public Suit Suit => (Suit)(value >> 5);

    public bool CanPlaceOn(Card other)
    {
        if (Suit != other.Suit)
        {
            return false;
        }

        return (Rank == other.Rank - 1) || (Rank == other.Rank + 1);
    }

    public override string ToString() => Suit == Suit.Arcana
        ? Rank.ToString()
        : rankStrings[Rank - 2] + suitString[(int)Suit];

    public static IEnumerable<Card> CreateDeck()
    {
        for (var i = 0; i <= 21; i++)
        {
            yield return new Card(i, Suit.Arcana);
        }

        foreach (var suit in new[] { Suit.Red, Suit.Green, Suit.Blue, Suit.Yellow })
        {
            for (var i = 2; i <= 13; i++)
            {
                yield return new Card(i, suit);
            }
        }
    }

    public bool Equals(Card other) => value == other.value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Card other && Equals(other);

    public override int GetHashCode() => value;
}
