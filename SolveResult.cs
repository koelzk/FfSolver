
namespace FfSolver;

public class SolveResult
{
    public SolveResult(SolveResultStatus status, List<Move>? moves = null)
    {
        Moves = moves;
        Status = status;
    }

    public List<Move>? Moves { get; }

    public bool Solved => Status == SolveResultStatus.Solved;

    public SolveResultStatus Status { get; } 
}
