using System.Text;
using FfSolver;
using SolverApp;

public static class BoardDisplayHelper
{
    public static string ToColorString(Card card)
    {
        var foregroundColor = card.Suit switch
        {
            Suit.Red => AnsiColor.Red,
            Suit.Green => AnsiColor.Green,
            Suit.Blue => AnsiColor.Blue,
            Suit.Yellow => AnsiColor.Yellow,
            _ => AnsiColor.White,
        };

        var backgroundColor = card.Suit switch
        {
            Suit.MajorArc => AnsiColor.Black,
            _ => AnsiColor.White,
        };

        var styles = new[]
        {
            AnsiColorStringHelper.GetCommand(foregroundColor, AnsiStyle.Regular),
            //AnsiColorStringHelper.GetCommand(backgroundColor, AnsiStyle.Background),
        };

        var suitString = "♥♣♠♦ "[(int)card.Suit];

        return AnsiColorStringHelper.GetString($"{card.GetRankString() + suitString,-3} ", styles);
    }

    public static string ToColorString(Card? card) => card.HasValue
        ? ToColorString(card.Value)
        : " -- ";

    public static string ToColorString(Board board)
    {
        var sb = new StringBuilder();

        var fdnStrings = board.MinorArcFoundations.Select(c => ToColorString(c));

        var arcanaLowFdnString = ToColorString(board.MajorArcFoundationLow);
        var arcanaHighFdnString = ToColorString(board.MajorArcFoundationHigh);
        sb.AppendLine($"{arcanaLowFdnString,4} {arcanaHighFdnString,4}     {ToColorString(board.Cell)}       {string.Join(" ", fdnStrings)}");

        var maxCount = board.Cascades.Select(cc => cc.Count).Max();
        for (var row = 0; row < maxCount; row++)
        {
            var rowString = board.Cascades.Select(cc => row < cc.Count ? cc[row] : default(Card?))
                .Select(ToColorString);
            sb.AppendLine(string.Join(" ", rowString));
        }

        return sb.ToString();
    }
}
