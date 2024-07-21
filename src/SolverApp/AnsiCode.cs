using System.Text;

namespace SolverApp;

public static class AnsiColorStringHelper
{
    private const string Prefix = "\x1b[";
    private const string Postfix = @"m";

    private const string ResetCommand = $"{Prefix}0{Postfix}";

    public static string GetCommand(AnsiColor color, string style = AnsiStyle.Regular)
    {
        return $"{Prefix}{style}{(char)color}{Postfix}";
    }

    public static string GetString(string text, params string[] commands)
    {
        var sb = commands.Aggregate(new StringBuilder(), (sb, command) => sb.Append(command));
        sb.Append(text);
        sb.Append(ResetCommand);

        return sb.ToString();
    }
}

public static class AnsiStyle
{
    public const string Regular = "0;3";
    public const string Bold = "1;3";
    public const string Intense = "1;9";
    public const string Background = "4";
}

public enum AnsiColor
{
    Black = '0',
    Red = '1',
    Green = '2',
    Yellow = '3',
    Blue = '4',
    Purple = '5',
    Cyan = '6',
    White = '7',
}
