using FfSolver;

static void Solve()
{
    //var seed = 994;
    var results = new List<SolveResult>();

    var maxCount = 1000;
    Parallel.For(0, maxCount, seed =>
    {
        var board = Board.CreateRandom(new Random(seed));
        //Console.WriteLine(board);

        var solver = new Solver(board);
        var result = solver.Solve(maxSteps: 200);

        lock (results)
        {
            results.Add(result);
        }

        if (result.Solved)
        {
            Console.WriteLine(seed);
        }

        // Console.WriteLine($"{seed, 5}\t" + (result.Solved ? $"{result.Moves?.Count}" : result.Status.ToString()));
    });

    Console.WriteLine($"Iteration Sum: {results.Select(r => r.Iteration).Sum()}");
    Console.WriteLine($"Moves Sum: {results.Select(r => r.Moves?.Count ?? 0).Sum()}");
    Console.WriteLine($"Solved: {results.Select(r => r.Solved ? 1 : 0).Sum()}");
}

//BoardExtractor.ExtractImageTile(@"/home/konrad/Desktop/FfSolver/image.png");
var be = new BoardExtractor();
var board = be.DetectBoard(@"/home/konrad/Desktop/FfSolver/image.png");
var solver = new Solver(board);
var result = solver.Solve(maxSteps: 70);

foreach (var (move, index) in result.Moves.Select(Tuple.Create<Move, int>))
{
    System.Console.WriteLine($"{index + 1}. {move}");

}


//Solve();
return;

// var cascades = Enumerable.Range(0, 11).Select(i => new List<Card> { new Card(i, Suit.Arcana) }).ToList();
// cascades[0].Add(new Card(Card.QueenRank, Suit.Blue));
// cascades[0].Add(new Card(Card.KingRank, Suit.Blue));
// var board = new Board(cascades);

// board = Board.CreateRandom(new Random(1));

// // var boardString = @"13 5Y 3B 6G QG - 6B 21 QB 3G 10
// // KG  2 QR 4R 3Y - 8Y 10G 7G 3R 4Y
// // 5R 9 2Y KB 5B - JR 1 19 11 6R
// // 9Y 4B 10Y 8G 8B - JG 4 0 QY 2B
// // 16 7 7B 5G 8 - 4G 2G 2R 6Y 14
// // JY 15 KY 9R 18 - 3 8R 7Y 20 12
// // 9B KR 10R 6 7R - 5 17 JB 10B 9G";
// // board = BoardHelper.Parse(boardString);

// Console.WriteLine(board);

// var solver = new Solver(board);
// var result = solver.Solve();
// Console.WriteLine($"Result Status: {result.Status}");

// var b = new Board(board);
// var moves = (result.Moves ?? Enumerable.Empty<Move>()).Select(Tuple.Create<Move, int>);
// foreach (var (move, i) in moves)
// {
//     b.ApplyMove(move);
//     b.ApplyAutoMoves();
//     Console.WriteLine($"{i + 1}: {move}:\n{b}\n");
// }