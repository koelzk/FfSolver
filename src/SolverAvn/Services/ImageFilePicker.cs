using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace SolverAvn.Services;

public class ImageFilePicker : IImageFilePicker
{

    public async Task<string?> OpenFileDialogAsync()
    {
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : throw new InvalidOperationException("Application has no main window.");

        var topLevel = TopLevel.GetTopLevel(mainWindow) ?? throw new InvalidOperationException();

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        return files.Count > 0
            ? files[0].Path.LocalPath
            : null;
    }

}
