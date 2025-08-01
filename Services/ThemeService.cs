using Microsoft.JSInterop;

namespace PomodoroFocus.Services
{
    public class ThemeService : IThemeService
    {
        public async Task SetThemeAsync(string theme, IJSRuntime jsRuntime)
        {
            // 检查传入的 jsRuntime 是否有效
            if (jsRuntime == null)
            {
                throw new ArgumentNullException(nameof(jsRuntime), "A valid IJSRuntime instance must be provided.");
            }

            // 使用调用者传递过来的、有效的 jsRuntime 实例
            await jsRuntime.InvokeVoidAsync("setTheme", theme);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
        }
    }
}
