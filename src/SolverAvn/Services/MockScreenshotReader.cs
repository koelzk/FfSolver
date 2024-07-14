using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using FfSolver;

namespace SolverAvn.Services;

public class MockScreenshotReader : IScreenshotReader
{
    public ScreenshotReaderResult ReadScreenshot(string imageFilePath)
    {
        var screenshot = new RenderTargetBitmap(new PixelSize(1920, 1080));

        using var dc = screenshot.CreateDrawingContext();
        dc.DrawEllipse(new SolidColorBrush(0xFFFF0000), new Pen(), new Rect(screenshot.Size));

        return new ScreenshotReaderResult(
            screenshot,
            BoardHelper.CreateRandomFromSeed(1337),
            new List<DetectedCard>
            {
                new DetectedCard(new Rect(178, 347, 118, 29), new Card(Card.JackRank, Suit.Green)),
                new DetectedCard(new Rect(178, 377, 118, 29), new Card(7, Suit.Red)),
                new DetectedCard(new Rect(178, 347, 118, 29), new Card(10, Suit.Yellow)),
            });
    }
}