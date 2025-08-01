using Microsoft.JSInterop;

namespace PomodoroFocus.Services
{
    public interface IThemeService
    {
        Task SetThemeAsync(string theme, IJSRuntime jsRuntime);
    }
}
