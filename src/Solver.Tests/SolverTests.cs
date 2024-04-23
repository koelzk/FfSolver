namespace FfSolver.Tests;

public class SolverTests
{
    [Fact]
    public void TestSolve()
    {
        // Arrange
        var boardString = @"13 5Y 3B 6G QG - 6B 21 QB 3G 10
KG  2 QR 4R 3Y - 8Y 10G 7G 3R 4Y
5R 9 2Y KB 5B - JR 1 19 11 6R
9Y 4B 10Y 8G 8B - JG 4 0 QY 2B
16 7 7B 5G 8 - 4G 2G 2R 6Y 14
JY 15 KY 9R 18 - 3 8R 7Y 20 12
9B KR 10R 6 7R - 5 17 JB 10B 9G";

        var board = BoardHelper.Parse(boardString);
        var solver = new Solver(board);

        // Act
        var result = solver.Solve(maxIterations: 200_000, maxSteps: 70);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Solved);
        Assert.Equal(SolveResultStatus.Solved, result.Status);

        Assert.NotNull(result.Moves);
        Assert.True(result.Moves.Count < 70);
    }

    [Fact]
    public void TestSolve2()
    {
        // Arrange
        var boardString = @"10	11	KR	6	QG	-	7Y	6Y	QR	JR	20
5	1	4	JG	5G	-	7B	2Y	15	5Y	7G
8Y	3R	5B	2G	18	-	6G	19	JB	4Y	21
9	KB	KG	3Y	KY	-	8R	9B	14	6B	2B
0	2R	5R	QY	2	-	4B	4G	10Y	6R	9R
8B	3	12	7R	7	-	13	9Y	10R	QB	17
8	10B	10G	4R	16	-	8G	JY	9G	3B	3G";

        var board = BoardHelper.Parse(boardString);
        var solver = new Solver(board);

        // Act
        var result = solver.Solve(maxIterations: 200_000, maxSteps: 70);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Solved);
        Assert.Equal(SolveResultStatus.Solved, result.Status);

        Assert.NotNull(result.Moves);
        Assert.True(result.Moves.Count < 70);
    }
}