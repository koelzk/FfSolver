namespace SolverAvn.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media;
using FfSolver;
using ReactiveUI;
using SolverAvn.Services;

public class MainWindowViewModel : ViewModelBase
{
    protected readonly IScreenshotReader ScreenshotReader;
    protected readonly IBoardSolver BoardSolver;
    protected readonly IImageFilePicker ImageFilePicker;

    private ScreenshotReaderResult? readResult = null;
    private SolverResult? solverResult = null;

    private bool isWorking = false;
    private IReadOnlyCollection<CardViewModel> cards = new List<CardViewModel>();

    public MainWindowViewModel(
        IScreenshotReader screenshotReader,
        IBoardSolver boardSolver,
        IImageFilePicker imageFilePicker)
    {
        ScreenshotReader = screenshotReader;
        BoardSolver = boardSolver;
        ImageFilePicker = imageFilePicker;

        ReadScreenshotCommand = CommandHelper.Create(ReadScreenshot);
        SolveCommand = CommandHelper.Create(
            Solve,
            this.WhenAnyValue(vm => vm.CanSolve, value => value == true));
    }

    public bool CanSolve => ReadResult?.Board != null;

    public IImage? Screenshot => ReadResult?.Screenshot;

    public IReadOnlyCollection<CardViewModel> Cards
    {
        get => cards;
        protected set => this.RaiseAndSetIfChanged(ref cards, value);
    }

    public IReadOnlyCollection<Move> Moves => SolverResult?.Moves ?? Array.Empty<Move>();

    public bool IsWorking
    {
        get => isWorking;
        private set => this.RaiseAndSetIfChanged(ref isWorking, value);
    }

    public bool HasSolverResult => SolveResultStatus != null;

    public SolveResultStatus? SolveResultStatus => SolverResult?.Status;

    public ReactiveCommand<Unit, Unit> ReadScreenshotCommand { get; }

    public ReactiveCommand<Unit, Unit> SolveCommand { get; }

    protected ScreenshotReaderResult? ReadResult
    {
        get => readResult;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(ReadResult));
            }

            if (value == readResult)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref readResult, value);
            Cards = value.Cards.Select(c => new CardViewModel(c)).ToList();
            this.RaisePropertyChanged(nameof(Screenshot));
            this.RaisePropertyChanged(nameof(CanSolve));
        }
    }

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

        SolverResult = await PerformWork(() => BoardSolver.SolveBoard(board));
    }

    private async Task ReadScreenshot()
    {
        var imageFilePath = await ImageFilePicker.OpenFileDialogAsync();
        if (imageFilePath == null)
        {
            return;
        }

        ReadResult = await PerformWork(() => ScreenshotReader.ReadScreenshot(imageFilePath));
    }

    private async Task<T> PerformWork<T>(Func<T> action)
    {
        try
        {
            IsWorking = true;
            return await Task.Run(action);
        }
        finally
        {
            IsWorking = false;
        }
    }
}

public class DesignTimeMainWindowViewModel : MainWindowViewModel
{
    public DesignTimeMainWindowViewModel()
        : base(new MockScreenshotReader(), new MockBoardSolver(), new MockImageFilePicker())
    {
        ReadResult = ScreenshotReader.ReadScreenshot("");
        SolverResult = BoardSolver.SolveBoard(ReadResult.Board);
    }
}
