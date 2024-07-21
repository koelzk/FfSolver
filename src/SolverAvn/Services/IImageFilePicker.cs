using System.Threading.Tasks;

namespace SolverAvn.Services;

public interface IImageFilePicker
{
    public Task<string?> OpenFileDialogAsync();
}
