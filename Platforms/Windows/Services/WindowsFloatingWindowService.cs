#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using PomodoroFocus.Services;
using Windows.Graphics;
using Microsoft.Maui.Devices; // 需要引用此命名空间以获取屏幕密度
using Windows.Foundation;     // 需要引用此命名空间以处理事件

namespace PomodoroFocus.Platforms.Windows.Services
{
    public class WindowsFloatingWindowService : IFloatingWindowService
    {
        public event Action OnWindowClosed;

        private Window _widgetWindow;
        private readonly IServiceProvider _serviceProvider;

        public WindowsFloatingWindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Show()
        {
            if (_widgetWindow != null)
            {
                // 如果窗口已存在，激活它
                var nativeWindow = _widgetWindow.Handler.PlatformView as Microsoft.UI.Xaml.Window;
                nativeWindow?.Activate();
                return;
            }

            var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
            var settings = settingsService.CurrentSettings;

            // 1. 创建 MAUI 窗口
            var widgetPage = _serviceProvider.GetRequiredService<WidgetPage>();
            _widgetWindow = new Window(widgetPage);
            Application.Current.OpenWindow(_widgetWindow);

            // 2. 获取底层的 WinUI 3 AppWindow 对象
            var nativeWindowObj = _widgetWindow.Handler.PlatformView as Microsoft.UI.Xaml.Window;
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindowObj);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            // 3. 关键修改：配置无标题栏窗口
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            // 使用 OverlappedPresenter 替代 CompactOverlay，以获得完全控制权
            var presenter = OverlappedPresenter.Create();
            presenter.IsAlwaysOnTop = true;  // 保持置顶 (悬浮窗特性)
            presenter.IsResizable = true;    // 【解决问题1】允许调整大小
            presenter.IsMaximizable = false; // 不需要最大化
            presenter.IsMinimizable = false;

            appWindow.SetPresenter(presenter);

            // 4. 设置初始大小 (使用屏幕密度计算)
            var density = DeviceDisplay.MainDisplayInfo.Density;
            int width = settings.WidgetWidth > 0 ? settings.WidgetWidth : 260;
            int height = settings.WidgetHeight > 0 ? settings.WidgetHeight : 180;
            appWindow.Resize(new SizeInt32((int)(width * density), (int)(height * density)));

            TypedEventHandler<AppWindow, AppWindowChangedEventArgs> changedHandler = (sender, args) =>
            {
                // 当窗口大小发生变化时
                if (args.DidSizeChange)
                {
                    // 将物理像素转回逻辑像素存储，这样换了屏幕 DPI 也不会突变
                    var currentDensity = DeviceDisplay.MainDisplayInfo.Density;
                    settings.WidgetWidth = (int)(appWindow.Size.Width / currentDensity);
                    settings.WidgetHeight = (int)(appWindow.Size.Height / currentDensity);
                }
            };

            appWindow.Changed += changedHandler;

            // 清理与保存
            _widgetWindow.Destroying += async (s, e) =>
            {
                appWindow.Changed -= changedHandler;
                _widgetWindow = null;

                // 窗口关闭时，将最终的大小保存到硬盘
                await settingsService.SaveSettingsAsync();

                OnWindowClosed?.Invoke();
            };
        }

        public void Hide()
        {
            if (_widgetWindow != null)
            {
                Application.Current.CloseWindow(_widgetWindow);
                _widgetWindow = null;
            }
        }
    }
}
#endif