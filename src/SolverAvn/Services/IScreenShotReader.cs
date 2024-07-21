namespace SolverAvn.Services;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FfSolver;

public interface IScreenshotReader
{
    ScreenshotReaderResult ReadScreenshot(string imageFilePath);
}

public record DetectedCard(Rect Region, Card Card);

public record ScreenshotReaderResult(IImage Screenshot, Board Board, IReadOnlyCollection<DetectedCard> Cards);
