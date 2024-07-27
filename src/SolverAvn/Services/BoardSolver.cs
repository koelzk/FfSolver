namespace SolverAvn.Services;

using System;
using System.Threading;
using FfSolver;

public class BoardSolver : IBoardSolver
{
    private int maxIterations;
    private int maxSteps;

    public BoardSolver(int maxIterations, int maxSteps)
    {
        this.maxIterations = maxIterations;
        this.maxSteps = maxSteps;
    }

    public SolverResult SolveBoard(Board board, IProgress<BoardSolveProgress> progress, CancellationToken cancellationToken)
    {
        var solver = new Solver(board);

        var moveCount = int.MaxValue;
        var statusText = "Finding a solution...";

        var result = solver.Solve(p =>
        {
            if (p.Moves?.Count < moveCount)
            {
                moveCount = p.Moves.Count;
                statusText = $"Found a solution with {moveCount} moves.";
            }

            var solverProgress = new BoardSolveProgress(p.Iteration * 100.0f / maxIterations, statusText);
            progress.Report(solverProgress);
        }, maxIterations, maxSteps, cancellationToken);

        return new SolverResult(result.Moves ?? Array.Empty<Move>(), result.Status);
    }
}
