using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using FfSolver;

namespace SolverAvn.Services;

public class MockScreenshotReader : IScreenshotReader
{
    private static readonly int[] XCoords = [178, 323, 467, 612, 757, 1046, 1191, 1336, 1480, 1625];
    private static readonly int[] YCoords = [347, 378, 409, 440, 471, 502, 533];

    public ScreenshotReaderResult ReadScreenshot(string imageFilePath)
    {
        var screenshot = new RenderTargetBitmap(new PixelSize(1920, 1080));

        using var dc = screenshot.CreateDrawingContext();
        dc.FillRectangle(new SolidColorBrush(0xFF808080), new Rect(screenshot.Size));

        var board = BoardHelper.CreateRandomFromSeed(1337);

        var cards = YCoords.SelectMany((y, j) => XCoords.Select((x, i) =>
            new DetectedCard(new Rect(x, y, 118, 29), board.Cascades[i > 4 ? i + 1 : i][j])));

        return new ScreenshotReaderResult(
            screenshot,
            board,
            cards.ToList());
    }
}
