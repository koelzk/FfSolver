namespace SolverAvn.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using FfSolver;
using ReactiveUI;
using SolverAvn.Services;

/// <summary>
/// View model for main window
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    protected readonly IScreenshotReader ScreenshotReader;
    protected readonly IBoardSolver BoardSolver;
    protected readonly IImageFilePicker ImageFilePicker;

    protected CancellationTokenSource cts = new CancellationTokenSource();

    private ScreenshotReaderResult? readResult = null;
    private SolverResult? solverResult = null;

    private bool isWorking = false;
    private IReadOnlyCollection<CardViewModel> cards = new List<CardViewModel>();
    private bool canCancel;
    private string statusText = "";

    /// <summary>
    /// Initializes a new instance of <see cref="MainWindowViewModel"/>.
    /// </summary>
    /// <param name="screenshotReader">Reads board states from screenshot images</param>
    /// <param name="boardSolver">Solves boards</param>
    /// <param name="imageFilePicker">Provides image file picking</param>
    public MainWindowViewModel(
        IScreenshotReader screenshotReader,
        IBoardSolver boardSolver,
        IImageFilePicker imageFilePicker)
    {
        ScreenshotReader = screenshotReader;
        BoardSolver = boardSolver;
        ImageFilePicker = imageFilePicker;

        ReadScreenshotCommand = CommandHelper.Create(
            ReadScreenshot,
            this.WhenAnyValue(vm => vm.IsWorking, isWorking => !isWorking));
        SolveCommand = CommandHelper.Create(
            Solve,
            this.WhenAnyValue(vm => vm.CanSolve, vm => vm.IsWorking, (canSolve, isWorking) => canSolve && !isWorking));

        CancelCommand = ReactiveCommand.Create(
            Cancel,
            this.WhenAnyValue(vm => vm.CanCancel, (bool canCancel) => canCancel));
    }

    /// <summary>
    /// Indicates if board solving can be started.
    /// </summary>
    public bool CanSolve => ReadResult?.Board != null;

    /// <summary>
    /// Gets the loaded screenshot.
    /// </summary>
    public IImage? Screenshot => ReadResult?.Screenshot;

    /// <summary>
    /// Gets the list of detected cards for display in the view.
    /// </summary>
    public IReadOnlyCollection<CardViewModel> Cards
    {
        get => cards;
        protected set => this.RaiseAndSetIfChanged(ref cards, value);
    }

    /// <summary>
    /// Gets the move instructions returned by the solver as list of strings.
    /// </summary>
    public IReadOnlyCollection<string> Moves { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Gets if the view model is currently busy with an long-running operation (either board detection or solving).
    /// </summary>
    /// <value></value>
    public bool IsWorking
    {
        get => isWorking;
        protected set => this.RaiseAndSetIfChanged(ref isWorking, value);
    }

    /// <summary>
    /// Gets if the current ongoing operation can be canceled.
    /// </summary>
    public bool CanCancel
    {
        get => canCancel;
        protected set => this.RaiseAndSetIfChanged(ref canCancel, value);
    }

    /// <summary>
    /// Gets if the solver returned a result.
    /// </summary>
    public bool HasSolverResult => SolveResultStatus != null;

    public SolveResultStatus? SolveResultStatus => SolverResult?.Status;

    /// <summary>
    /// Gets the command for reading a board state from a screenshot.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ReadScreenshotCommand { get; }

    /// <summary>
    /// Gets the command for solving a board.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SolveCommand { get; }

    /// <summary>
    /// Gets the comman for canceling an on-going operation.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    /// <summary>
    /// Gets the progress in percent of an on-going operation.
    /// </summary>
    public float ProgressInPercent { get; protected set; } = 0f;

    /// <summary>
    /// Gets the current status of an ongoing operation.
    /// </summary>
    public string StatusText
    {
        get => statusText;
        protected set => this.RaiseAndSetIfChanged(ref statusText, value);
    }

    /// <summary>
    /// Gets or sets the result of board detection.
    /// </summary>
    protected ScreenshotReaderResult? ReadResult
    {
        get => readResult;
        set
        {
            if (value == readResult || value == null)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref readResult, value);
            Cards = value.Cards.Select(dc => new CardViewModel(dc)).ToList();
            this.RaisePropertyChanged(nameof(Screenshot));
            this.RaisePropertyChanged(nameof(CanSolve));
        }
    }

    /// <summary>
    /// Gets or sets the board solving result.
    /// </summary>
    protected SolverResult? SolverResult
    {
        get => solverResult;
        set
        {
            if (solverResult == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref solverResult, value);
            this.RaisePropertyChanged(nameof(HasSolverResult));
            this.RaisePropertyChanged(nameof(SolveResultStatus));

            Moves = (SolverResult?.Moves ?? Enumerable.Empty<Move>())
                .Select((m, i) => $"{i + 1}. {m}")
                .ToList();
            this.RaisePropertyChanged(nameof(Moves));
        }
    }

    private async Task Solve()
    {
        var board = ReadResult?.Board;

        if (board == null)
        {
            return;
        }

        var progress = new Progress<BoardSolveProgress>(p => UpdateProgress(p.Percent, p.StatusText));

        SolverResult = await PerformWork(ct => BoardSolver.SolveBoard(board, progress, ct), canCancel: true);

        if (SolverResult?.Status == FfSolver.SolveResultStatus.NoSolution)
        {
            StatusText = "Board has no solution.";
        }
        if (SolverResult?.Status == FfSolver.SolveResultStatus.ReachedMaxIterations)
        {
            StatusText = "Could not find solution.";
        }
    }

    private async Task ReadScreenshot()
    {
        var imageFilePath = await ImageFilePicker.OpenFileDialogAsync();
        if (imageFilePath == null)
        {
            return;
        }

        ReadResult = await PerformWork(_ => ScreenshotReader.ReadScreenshot(imageFilePath));
        StatusText = "Image loaded.";
    }

    private void Cancel()
    {
        cts.Cancel();
        CanCancel = false;
    }

    private async Task<T?> PerformWork<T>(Func<CancellationToken, T> action, bool canCancel = false) where T : class
    {
        try
        {
            IsWorking = true;
            CanCancel = canCancel;

            if (canCancel)
            {
                cts = new CancellationTokenSource();
            }
            return await Task.Run(() => action(cts.Token));
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            return null;
        }
        finally
        {
            CanCancel = false;
            IsWorking = false;
        }
    }

    private void UpdateProgress(float progressInPercent, string statusText)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusText = statusText;
            ProgressInPercent = progressInPercent;

            this.RaisePropertyChanged(nameof(StatusText));
            this.RaisePropertyChanged(nameof(ProgressInPercent));
        }, DispatcherPriority.Background);
    }
}

/// <summary>
/// Derived view model intended for design-time preview only
/// </summary>
public class DesignTimeMainWindowViewModel : MainWindowViewModel
{
    public DesignTimeMainWindowViewModel()
        : base(new MockScreenshotReader(), new MockBoardSolver(), new MockImageFilePicker())
    {
        ReadResult = ScreenshotReader.ReadScreenshot("");
        SolverResult = BoardSolver.SolveBoard(ReadResult.Board, new Progress<BoardSolveProgress>(p => { }));
        IsWorking = true;
        ProgressInPercent = 76;
        StatusText = "Calculating solution...";
    }
}
