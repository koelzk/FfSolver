namespace SolverAvn.Services;

using System.Linq;
using FfSolver;

public class MockBoardSolver : IBoardSolver
{
    public SolverResult SolveBoard(Board board)
    {
        var moves = Enumerable.Range(0, 60)
            .Select(i => new Move(i % 10, (i % 10) - 1))
            .ToList();

        return new SolverResult(moves, SolveResultStatus.Solved);
    }
}