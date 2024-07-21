namespace FfSolver;

public readonly struct Move
{
    public const int Cell = -1;
    public const int Foundation = -2;

    public Move(int from, int to) : this(from, to, 1)
    {
        // Nothing to do here.
    }

    public Move(int from, int to, int count)
    {
        From = from > Foundation && from < 11 ? from : throw new ArgumentOutOfRangeException(nameof(from));
        To = to >= Foundation && to < 11 && to != from ? to : throw new ArgumentOutOfRangeException(nameof(to));
        Count = count > 0 ? count : throw new ArgumentOutOfRangeException(nameof(count));
    }

    public int From { get; }

    public int To { get; }

    public int Count { get; }

    public override string ToString() => Count == 1
        ? $"Move card from {GetLocationString(From)} to {GetLocationString(To)}"
        : $"Move {Count} cards from {GetLocationString(From)} to {GetLocationString(To)}";

    private string GetLocationString(int index) =>
        index switch
        {
            Cell => "cell",
            Foundation => "foundation",
            _ => $"cascade {index}",
        };

}
