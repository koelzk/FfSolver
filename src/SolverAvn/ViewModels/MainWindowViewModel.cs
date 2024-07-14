using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using SolverAvn.Services;

namespace SolverAvn.ViewModels;

public class MainWindowViewModel(IScreenshotReader screenshotReader) : ViewModelBase
{
    protected IScreenshotReader ScreenshotReader = screenshotReader;

    protected ScreenshotReaderResult? Result = null;

    public string Greeting { get; set; } = "Welcome to Avalonia!";

    public IImage? Screenshot => Result?.Screenshot;

    public IReadOnlyCollection<DetectedCard> Cards =>
        Result?.Cards ?? Array.Empty<DetectedCard>();
}

public class DesignTimeMainWindowViewModel : MainWindowViewModel
{
    public DesignTimeMainWindowViewModel() : base(new MockScreenshotReader())
    {
        Greeting = "Mock";
        Result = ScreenshotReader.ReadScreenshot("");
    }
}
