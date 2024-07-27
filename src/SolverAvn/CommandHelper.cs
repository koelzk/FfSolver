using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;

namespace SolverAvn.ViewModels;

public static class CommandHelper
{
    public static ReactiveCommand<Unit, Unit> Create(Func<Task> taskFactory, IObservable<bool>? canExecute = null, IScheduler? outputScheduler = null)
    {
        return ReactiveCommand.Create(() => Dispatcher.UIThread.Post(() => taskFactory(), DispatcherPriority.Background), canExecute, outputScheduler);
    }
}

