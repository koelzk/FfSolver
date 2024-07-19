using System.Threading.Tasks;

namespace SolverAvn.Services;

public class MockImageFilePicker : IImageFilePicker
{
    public Task<string?> OpenFileDialogAsync() => 
        Task.FromResult<string?>(@"X:\Some\Path\To\An\ImageFile.jpg");
}
