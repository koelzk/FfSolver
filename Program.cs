using FfSolver;

var seed = 994;
var board = Board.CreateRandom(new Random(seed));
Console.WriteLine(board);

while (true)
{
    var move = board.EnumerateAutoMoves().Cast<Move?>().FirstOrDefault();

    if (move.HasValue)
    {
        board.ApplyMove(move.Value);
        Console.WriteLine(board);
    }
    else
    {
        break;
    }
}