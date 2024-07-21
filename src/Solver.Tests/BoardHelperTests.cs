using System.Diagnostics;

namespace FfSolver.Tests;

public class BoardHelperTests
{
    [Fact]
    public void TestCreateRandomFromSeed()
    {
        var board1 = BoardHelper.CreateRandomFromSeed(1);
        var board2 = BoardHelper.CreateRandomFromSeed(2);
        var board1Copy = BoardHelper.CreateRandomFromSeed(1);
        var board2Copy = BoardHelper.CreateRandomFromSeed(2);

        Assert.Equal(board1, board1Copy);
        Assert.Equal(board2, board2Copy);
        Assert.NotEqual(board1, board2);
        Assert.NotEqual(board1, board2Copy);
        Assert.NotEqual(board2, board1Copy);
    }

    [Fact]
    public void TestParse()
    {
        // Given
        var boardString = @"
            13 5Y  3B 6G QG - 6B  21 QB  3G 10
            KG  2  QR 4R 3Y - 8Y 10G 7G  3R 4Y
            5R  9  2Y KB 5B - JR   1 19  11 6R 
            9Y 4B 10Y 8G 8B - JG   4  0  QY 2B
            16  7  7B 5G  8 - 4G  2G 2R  6Y 14
            JY 15  KY 9R 18 -  3  8R 7Y  20 12
            9B KR 10R  6 7R -  5  17 JB 10B 9G";

        // When
        var board = BoardHelper.Parse(boardString);

        // Then
        Assert.Equal(new Card(13, Suit.MajorArc), board.Cascades[0][0]);
        Assert.Equal(new Card(Card.KingRank, Suit.Green), board.Cascades[0][1]);
        Assert.Equal(new Card(5, Suit.Red), board.Cascades[0][2]);
        Assert.Equal(new Card(9, Suit.Yellow), board.Cascades[0][3]);
        Assert.Equal(new Card(16, Suit.MajorArc), board.Cascades[0][4]);
        Assert.Equal(new Card(Card.JackRank, Suit.Yellow), board.Cascades[0][5]);
        Assert.Equal(new Card(9, Suit.Blue), board.Cascades[0][6]);

        Assert.Equal(new Card(5, Suit.Yellow), board.Cascades[1][0]);
        Assert.Equal(new Card(2, Suit.MajorArc), board.Cascades[1][1]);
        Assert.Equal(new Card(9, Suit.MajorArc), board.Cascades[1][2]);
        Assert.Equal(new Card(4, Suit.Blue), board.Cascades[1][3]);
        Assert.Equal(new Card(7, Suit.MajorArc), board.Cascades[1][4]);
        Assert.Equal(new Card(15, Suit.MajorArc), board.Cascades[1][5]);
        Assert.Equal(new Card(Card.KingRank, Suit.Red), board.Cascades[1][6]);

        Assert.Equal(new Card(3, Suit.Blue), board.Cascades[2][0]);
        Assert.Equal(new Card(Card.QueenRank, Suit.Red), board.Cascades[2][1]);
        Assert.Equal(new Card(2, Suit.Yellow), board.Cascades[2][2]);
        Assert.Equal(new Card(10, Suit.Yellow), board.Cascades[2][3]);
        Assert.Equal(new Card(7, Suit.Blue), board.Cascades[2][4]);
        Assert.Equal(new Card(Card.KingRank, Suit.Yellow), board.Cascades[2][5]);
        Assert.Equal(new Card(10, Suit.Red), board.Cascades[2][6]);

        Assert.Equal(new Card(6, Suit.Green), board.Cascades[3][0]);
        Assert.Equal(new Card(4, Suit.Red), board.Cascades[3][1]);
        Assert.Equal(new Card(Card.KingRank, Suit.Blue), board.Cascades[3][2]);
        Assert.Equal(new Card(8, Suit.Green), board.Cascades[3][3]);
        Assert.Equal(new Card(5, Suit.Green), board.Cascades[3][4]);
        Assert.Equal(new Card(9, Suit.Red), board.Cascades[3][5]);
        Assert.Equal(new Card(6, Suit.MajorArc), board.Cascades[3][6]);

        Assert.Equal(new Card(Card.QueenRank, Suit.Green), board.Cascades[4][0]);
        Assert.Equal(new Card(3, Suit.Yellow), board.Cascades[4][1]);
        Assert.Equal(new Card(5, Suit.Blue), board.Cascades[4][2]);
        Assert.Equal(new Card(8, Suit.Blue), board.Cascades[4][3]);
        Assert.Equal(new Card(8, Suit.MajorArc), board.Cascades[4][4]);
        Assert.Equal(new Card(18, Suit.MajorArc), board.Cascades[4][5]);
        Assert.Equal(new Card(7, Suit.Red), board.Cascades[4][6]);

        Assert.Equal(0, board.Cascades[5].Count);

        Assert.Equal(new Card(6, Suit.Blue), board.Cascades[6][0]);
        Assert.Equal(new Card(8, Suit.Yellow), board.Cascades[6][1]);
        Assert.Equal(new Card(Card.JackRank, Suit.Red), board.Cascades[6][2]);
        Assert.Equal(new Card(Card.JackRank, Suit.Green), board.Cascades[6][3]);
        Assert.Equal(new Card(4, Suit.Green), board.Cascades[6][4]);
        Assert.Equal(new Card(3, Suit.MajorArc), board.Cascades[6][5]);
        Assert.Equal(new Card(5, Suit.MajorArc), board.Cascades[6][6]);

        Assert.Equal(new Card(21, Suit.MajorArc), board.Cascades[7][0]);
        Assert.Equal(new Card(10, Suit.Green), board.Cascades[7][1]);
        Assert.Equal(new Card(1, Suit.MajorArc), board.Cascades[7][2]);
        Assert.Equal(new Card(4, Suit.MajorArc), board.Cascades[7][3]);
        Assert.Equal(new Card(2, Suit.Green), board.Cascades[7][4]);
        Assert.Equal(new Card(8, Suit.Red), board.Cascades[7][5]);
        Assert.Equal(new Card(17, Suit.MajorArc), board.Cascades[7][6]);

        Assert.Equal(new Card(Card.QueenRank, Suit.Blue), board.Cascades[8][0]);
        Assert.Equal(new Card(7, Suit.Green), board.Cascades[8][1]);
        Assert.Equal(new Card(19, Suit.MajorArc), board.Cascades[8][2]);
        Assert.Equal(new Card(0, Suit.MajorArc), board.Cascades[8][3]);
        Assert.Equal(new Card(2, Suit.Red), board.Cascades[8][4]);
        Assert.Equal(new Card(7, Suit.Yellow), board.Cascades[8][5]);
        Assert.Equal(new Card(Card.JackRank, Suit.Blue), board.Cascades[8][6]);

        Assert.Equal(new Card(3, Suit.Green), board.Cascades[9][0]);
        Assert.Equal(new Card(3, Suit.Red), board.Cascades[9][1]);
        Assert.Equal(new Card(11, Suit.MajorArc), board.Cascades[9][2]);
        Assert.Equal(new Card(Card.QueenRank, Suit.Yellow), board.Cascades[9][3]);
        Assert.Equal(new Card(6, Suit.Yellow), board.Cascades[9][4]);
        Assert.Equal(new Card(20, Suit.MajorArc), board.Cascades[9][5]);
        Assert.Equal(new Card(10, Suit.Blue), board.Cascades[9][6]);

        Assert.Equal(new Card(10, Suit.MajorArc), board.Cascades[10][0]);
        Assert.Equal(new Card(4, Suit.Yellow), board.Cascades[10][1]);
        Assert.Equal(new Card(6, Suit.Red), board.Cascades[10][2]);
        Assert.Equal(new Card(2, Suit.Blue), board.Cascades[10][3]);
        Assert.Equal(new Card(14, Suit.MajorArc), board.Cascades[10][4]);
        Assert.Equal(new Card(12, Suit.MajorArc), board.Cascades[10][5]);
        Assert.Equal(new Card(9, Suit.Green), board.Cascades[10][6]);
    }

    [Fact(Skip = "Benchmark")]
    public void TestBenchmark()
    {
        var sw = Stopwatch.StartNew();
        var solved = 0;
        foreach (ulong seed in Enumerable.Range(0, 50))
        {
            var board = BoardHelper.CreateRandomFromSeed(seed);
            var solver = new Solver(board);
            var result = solver.Solve(100_000, 100, true);
            Console.WriteLine($"{seed} => {result.Status}");
            if (result.Solved)
            {
                solved++;
            }
        }
        sw.Stop();
        Console.WriteLine($"{solved} solved. Took {sw.Elapsed.TotalSeconds:F3} sec");
    }
}
