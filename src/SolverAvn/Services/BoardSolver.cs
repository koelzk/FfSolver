namespace SolverAvn.Services;

using System;
using FfSolver;

public class BoardSolver : IBoardSolver
{
    public SolverResult SolveBoard(Board board)
    {
        var solver = new Solver(board);
        var result = solver.Solve();

        return new SolverResult(result.Moves ?? Array.Empty<Move>(), result.Status);
    }
}
