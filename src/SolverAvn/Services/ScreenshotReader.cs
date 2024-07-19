namespace SolverAvn.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Media.Imaging;

public class ScreenshotReader : IScreenshotReader
{
    public ScreenshotReaderResult ReadScreenshot(string imageFilePath)
    {
        var executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var templatePath = Path.Combine(executablePath, "templates");

        var boardExtractor = new BoardExtractor(templatePath);
        var extractedCards = new List<ExtractedCard>();
        var board = boardExtractor.DetectBoard(imageFilePath, extractedCards);

        return new ScreenshotReaderResult(
            new Bitmap(imageFilePath),
            board,
            extractedCards.Select(e => new DetectedCard(
                new Avalonia.Rect(e.Region.Left, e.Region.Top, e.Region.Width, e.Region.Height),
                e.Card
            )).ToList());
    }
}