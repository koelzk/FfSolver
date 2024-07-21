namespace SolverAvn.Services;

using System.Collections.Generic;
using FfSolver;

public interface IBoardSolver
{
    SolverResult SolveBoard(Board board);
}

public record SolverResult(IReadOnlyList<Move> Moves, SolveResultStatus Status);
