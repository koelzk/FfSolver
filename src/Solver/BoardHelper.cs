using System.Text.RegularExpressions;

namespace FfSolver;

public static class BoardHelper
{
    private static readonly Regex cardRegex = new Regex(@"(?'rank'\d{1,2}|J|Q|K)(?'suit'[RGBY]?)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
}