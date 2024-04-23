using System.Reflection;
using FfSolver;

// Extract board state from image:
var appPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? string.Empty;
var imagePath = Path.Combine(appPath, "image.png");

var boardExtractor = new BoardExtractor(Path.Combine(appPath, "templates"));
var board = boardExtractor.DetectBoard(imagePath);

Console.WriteLine($"Extracted board state:\n{BoardDisplayHelper.ToColorString(board)}");

// Run solver:
var solver = new Solver(board);
var result = solver.Solve(maxIterations:10_000, maxSteps: 80);

if (!result.Solved)
{
    Console.WriteLine($"Solver could not find a solution ({result.Status}).");
}
else if (result.Moves is not null)
{
    Console.WriteLine($"Solver found a solution with {result.Moves.Count} moves:");

    var b = new Board(board);

    foreach (var (move, index) in result.Moves.Select(Tuple.Create<Move, int>))
    {
        Console.WriteLine(BoardDisplayHelper.ToColorString(b));
        Console.WriteLine($"{index + 1}. {move}\n");
        b.ApplyMove(move);
        b.ApplyAutoMoves();
    }
}
