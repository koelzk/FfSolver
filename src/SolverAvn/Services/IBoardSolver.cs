namespace SolverAvn.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using FfSolver;

public interface IBoardSolver
{
    SolverResult SolveBoard(Board board, IProgress<BoardSolveProgress> progress, CancellationToken cancellationToken = default);
}

public record BoardSolveProgress(float Percent, string StatusText);

public record SolverResult(IReadOnlyList<Move> Moves, SolveResultStatus Status);
