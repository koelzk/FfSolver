namespace FfSolver;

public readonly struct Move
{
    public const int Cell = -1;
    public const int Foundation = -2;

    public Move(int from, int to) : this(from, to, 1)
    {

    }

    public Move(int from, int to, int count)
    {
        From = from > Foundation && from < 11 ? from : throw new ArgumentOutOfRangeException(nameof(from));
        To = to >= Foundation && to < 11  && to != from ? to : throw new ArgumentOutOfRangeException(nameof(to));
        Count = count > 0 ? count : throw new ArgumentOutOfRangeException(nameof(count));
    }

    public int From { get; }

    public int To { get; }

    public int Count { get; }
}
