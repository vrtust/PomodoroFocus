#if WINDOWS
using PomodoroFocus.Services;
using System.Runtime.InteropServices;
using Application = Microsoft.Maui.Controls.Application;

namespace PomodoroFocus.Platforms.Windows.Services
{
    public class WindowsActivationService : IWindowActivationService
    {
        // 导入需要用到的 Win32 API 函数
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_RESTORE = 9;

        public void ActivateMainWindow()
        {
            // .NET MAUI 在 Windows 上运行时，Application.Current.Windows[0] 通常是主窗口
            if (Application.Current?.Windows is not null && Application.Current.Windows.Count > 0)
            {
                // 获取 MAUI 窗口对象
                var mauiWindow = Application.Current.Windows[0];
                // 从 MAUI 窗口的处理器中获取原生的 WinUI 窗口
                var nativeWindow = mauiWindow.Handler?.PlatformView as MauiWinUIWindow;

                if (nativeWindow is not null)
                {
                    // 从原生窗口获取窗口句柄 (HWND)
                    var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

                    // 如果窗口被最小化了 (IsIconic)，先恢复它
                    if (IsIconic(hWnd))
                    {
                        ShowWindow(hWnd, SW_RESTORE);
                    }

                    // 将窗口带到前台
                    SetForegroundWindow(hWnd);
                }
            }
        }
    }
}
#endif