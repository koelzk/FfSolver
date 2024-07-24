using System.Reflection;
using CommandLine;
using FfSolver;

public class Program
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Path to input file.")]
        public string InputFilePath { get; set; } = string.Empty;

        [Option('t', "text", Default = false, HelpText = "Read text instead of image from input file.")]
        public bool IsTextInput { get; set; }

        [Option('m', "max-iter", Default = 100_000, Required = false, HelpText = "Maximum number of iterations.")]
        public int MaxIterations { get; set; }

        [Option('s', "steps", Default = 80, Required = false, HelpText = "Maximum number of moves a solution may have.")]
        public int MaxSteps { get; set; }

        [Option('f', "full", Default = false, HelpText = "Evaluate all iterations to find best solution.")]
        public bool FullIteration { get; set; }

        [Option('b', "board", Default = true, Required = false, HelpText = "Display board for each move.")]
        public bool ShowBoard { get; set; } = true;
    }

    private static void Solve(Options options)
    {
        Board board;

        if (options.IsTextInput)
        {
            // Read board state from text file:
            board = BoardHelper.Parse(File.ReadAllText(options.InputFilePath));
        }
        else
        {
            // Read board state from image:
            var appPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? string.Empty;
            var boardExtractor = new BoardExtractor.BoardExtractor(Path.Combine(appPath, "templates"));
            board = boardExtractor.DetectBoard(options.InputFilePath);
        }

        Console.WriteLine($"Start board:\n{BoardDisplayHelper.ToColorString(board)}");

        // Run solver:
        var solver = new Solver(board);
        var result = solver.Solve(maxIterations: options.MaxIterations, maxSteps: options.MaxSteps, returnOnSolve: !options.FullIteration);

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
                if (options.ShowBoard)
                {
                    Console.WriteLine(BoardDisplayHelper.ToColorString(b));
                }

                Console.WriteLine($"{index + 1}. {move}\n");
                b.ApplyMove(move);
                b.ApplyAutoMoves();
            }
        }
    }

    static void Main(string[] args) =>
        Parser.Default.ParseArguments<Options>(args).WithParsed(Solve);
}
