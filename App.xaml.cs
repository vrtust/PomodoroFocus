namespace PomodoroFocus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 定义我们期望的窗口尺寸
            const int DesiredWidth = 1100;
            const int DesiredHeight = 650; // 给一个足够的高度以容纳所有内容

            // 创建主窗口
            var window = new Window(new MainPage())
            {
                Title = "PomodoroFocus"
            };

            window.Created += (s, e) =>
            {
                // 在这里调用平台特定代码来居中窗口
                CenterWindowOnScreen(window);
            };

            return window;
        }

        // App.xaml.cs

        private void CenterWindowOnScreen(Window window)
        {
#if WINDOWS
            // 获取原生 WinUI 窗口对象
            var nativeWindow = window.Handler.PlatformView as Microsoft.UI.Xaml.Window;
            if (nativeWindow == null) return;

            // 获取与此窗口关联的 AppWindow
            var appWindow = GetAppWindow(nativeWindow);
            if (appWindow == null) return;

            // 获取显示区域
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(appWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea == null) return;

            // 从显示区域获取工作区（不包括任务栏）
            var workArea = displayArea.WorkArea;

            // 获取窗口当前尺寸
            var windowWidth = appWindow.Size.Width;
            var windowHeight = appWindow.Size.Height;

            // 计算居中位置
            int x = workArea.X + (workArea.Width - windowWidth) / 2;
            int y = workArea.Y + (workArea.Height - windowHeight) / 2;

            // 移动窗口到新位置
            appWindow.Move(new Windows.Graphics.PointInt32(x, y));
#endif
        }

#if WINDOWS
        // 这是一个辅助方法，用于从 WinUI 窗口获取 AppWindow
        private Microsoft.UI.Windowing.AppWindow GetAppWindow(Microsoft.UI.Xaml.Window nativeWindow)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        }
#endif
    }
}
