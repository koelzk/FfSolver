using System.Reflection;
using FfSolver;

// Extract board state from image:
var executablePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? string.Empty;
var imagePath = Path.Combine(executablePath, "image.png");

var boardExtractor = new BoardExtractor();
var board = boardExtractor.DetectBoard(imagePath);

Console.WriteLine($"Extracted board state:\n{board}");

// Run solver:
var solver = new Solver(board);
var result = solver.Solve(maxSteps: 70);

if (!result.Solved)
{
    Console.WriteLine($"Solver could not find a solution ({result.Status}).");
}
else if (result.Moves is not null)
{
    Console.WriteLine($"Solver found a solution with {result.Moves.Count} moves:");

    foreach (var (move, index) in result.Moves.Select(Tuple.Create<Move, int>))
    {
        Console.WriteLine($"{index + 1}. {move}");
    }
}
