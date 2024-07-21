
namespace FfSolver;

public enum SolveResultStatus
{
    /// <summary>
    /// A solution was found.
    /// </summary>
    Solved,
    /// <summary>
    /// The maximum number of iterations was reached without finding a solution.
    /// </summary>
    ReachedMaxIterations,
    /// <summary>
    /// No solution exists with the number of moves less or equal to the specified maximum number of steps.
    /// </summary>
    NoSolution
}
