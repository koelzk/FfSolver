namespace FfSolver.Tests;

public class SolverTests
{
    [Fact]
    public void TestSolve()
    {
        // Arrange
        var boardString = @"
            13 5Y  3B 6G QG - 6B  21 QB  3G 10
            KG  2  QR 4R 3Y - 8Y 10G 7G  3R 4Y
            5R  9  2Y KB 5B - JR   1 19  11 6R
            9Y 4B 10Y 8G 8B - JG   4  0  QY 2B
            16  7  7B 5G  8 - 4G  2G 2R  6Y 14
            JY 15  KY 9R 18 -  3  8R 7Y  20 12
            9B KR 10R  6 7R -  5  17 JB 10B 9G";

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
        var boardString = @"
            7B   -  3Y  6G  5B  KY 10R  QB   -  5G   -
            -   -  KR  7G  4B  QY  JB  KB   -  21   -
            -   -   -  8G   -  JY 10B   -   -  8B   -
            -   -   -  9G   - 10Y  9B   -   -  3B   -
            -   -   - 10G   -  9Y   -   -   -  2Y   -
            -   -   -  JG   -  8Y   -   -   -  6B   -
            -   -   -  QG   -  7Y   -   -   -  QR   -
            -   -   -  KG   -  6Y   -   -   -  JR   -
            -   -   -   -   -  5Y   -   -   -   -   -
            -   -   -   -   -  4Y   -   -   -   -   -";

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