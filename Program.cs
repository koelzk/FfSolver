using FfSolver;

static void Solve()
{
    //var seed = 994;
    for (var seed = 0; seed < 1000; seed++)
    {
        Console.WriteLine($"Seed: {seed}");

        var board = Board.CreateRandom(new Random(seed));
        Console.WriteLine(board);

        var solver = new Solver(board);
        var result = solver.Solve(maxSteps: 100);

        if (result.Solved)
        {
            Console.WriteLine($"Found soultion");
        }
    }
}

//Solve();

var cascades = Enumerable.Range(0, 11).Select(i => new List<Card> { new Card(i, Suit.Arcana) }).ToList();
cascades[0].Add(new Card(Card.QueenRank, Suit.Blue));
cascades[0].Add(new Card(Card.KingRank, Suit.Blue));
var board = new Board(cascades);

board = Board.CreateRandom(new Random(1337));

Console.WriteLine(board);

var solver = new Solver(board);
var result = solver.Solve();

foreach (var move in result.Moves ?? Enumerable.Empty<Move>())
{
    System.Console.WriteLine(move);
}