#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using PomodoroFocus.Services;
using Windows.Graphics;

namespace PomodoroFocus.Platforms.Windows.Services
{
    public class WindowsFloatingWindowService : IFloatingWindowService
    {
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
                // 如果窗口已存在，只需激活它
                // (更复杂的逻辑可以处理窗口最小化等情况)
                return;
            }

            // 从DI容器动态获取Page实例，确保其依赖被注入
            var widgetPage = _serviceProvider.GetRequiredService<WidgetPage>();
            _widgetWindow = new Window(widgetPage)
            {
                // 可以设置初始大小，但CompactOverlay会覆盖它
            };

            _widgetWindow.Destroying += (s, e) => _widgetWindow = null;

            Application.Current.OpenWindow(_widgetWindow);

            // 获取原生窗口并设置为 CompactOverlay (画中画) 模式
            var nativeWindow = _widgetWindow.Handler.PlatformView as MauiWinUIWindow;
            var appWindow = nativeWindow.AppWindow;

            var titleBar = appWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;

            if (appWindow.Presenter is OverlappedPresenter)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
            }

            // 设置一个固定的初始尺寸
            appWindow.Resize(new SizeInt32(200, 160));
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
