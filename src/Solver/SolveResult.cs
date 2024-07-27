
namespace FfSolver;

public class SolveResult
{
    public SolveResult(SolveResultStatus status, int iteration, IReadOnlyList<Move>? moves = null)
    {
        Moves = moves;
        Status = status;
        Iteration = iteration;
    }

    public IReadOnlyList<Move>? Moves { get; }

    public bool Solved => Status == SolveResultStatus.Solved;

    public int Iteration { get; }

    public SolveResultStatus Status { get; }
}
