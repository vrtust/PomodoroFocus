using Microsoft.Maui.Controls.Shapes;
using PomodoroFocus.Models;
using PomodoroFocus.Services;

namespace PomodoroFocus;

public partial class WidgetPage : ContentPage
{
    private readonly IPomodoroTimerService _timerService;

    // SVG 路径数据 (直接从你的 Tailwind 代码中提取)
    private const string IconPlay = "M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z";
    private const string IconPause = "M18 10a8 8 0 11-16 0 8 8 0 0116 0zM7 8a1 1 0 012 0v4a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v4a1 1 0 102 0V8a1 1 0 00-1-1z";

    // 颜色定义 (对应 XAML 资源)
    private Color WorkBgLight = Color.FromArgb("#fee2e2"); // Red100
    private Color WorkTextLight = Color.FromArgb("#991b1b"); // Red800
    private Color WorkBgDark = Color.FromArgb("#450a0a"); // Red900/30 approx
    private Color WorkTextDark = Color.FromArgb("#fca5a5"); // Red300

    private Color BreakBgLight = Color.FromArgb("#dcfce7"); // Green100
    private Color BreakTextLight = Color.FromArgb("#166534"); // Green800
    private Color BreakBgDark = Color.FromArgb("#052e16"); // Green900/30 approx
    private Color BreakTextDark = Color.FromArgb("#86efac"); // Green300

    private Color WorkDot = Color.FromArgb("#ef4444"); // Red500
    private Color BreakDot = Color.FromArgb("#22c55e"); // Green500

    public WidgetPage(IPomodoroTimerService timerService)
    {
        InitializeComponent();
        _timerService = timerService;
        _timerService.OnTick += OnTimerTick;
        _timerService.OnPhaseCompleted += OnPhaseCompleted;

        // 初始渲染
        UpdateUI();
    }

    private void OnTimerTick()
    {
        // 计时器每秒触发，这里只更新文本
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimeLabel.Text = _timerService.TimeLeft.ToString("mm\\:ss");
        });
    }

    private void OnPhaseCompleted(PomodoroSession session, PomodoroCycleState state)
    {
        // 阶段结束或状态改变时，进行全量 UI 更新
        MainThread.BeginInvokeOnMainThread(UpdateUI);
    }

    private void UpdateUI()
    {
        // 1. 更新时间文本
        TimeLabel.Text = _timerService.TimeLeft.ToString("mm\\:ss");

        // 2. 更新播放/暂停图标
        PlayPauseIcon.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(
            _timerService.IsRunning ? IconPause : IconPlay);

        // 3. 更新状态胶囊的样式 (颜色和文字)
        var isWork = _timerService.CurrentCycleState == PomodoroCycleState.Work;
        var isDark = Application.Current.RequestedTheme == AppTheme.Dark;

        PhaseLabel.Text = isWork ? "专注时间" : "休息时间";
        StatusDot.Color = isWork ? WorkDot : BreakDot;

        // 设置胶囊背景和文字颜色
        if (isWork)
        {
            StatusBadge.BackgroundColor = isDark ? WorkBgDark : WorkBgLight;
            PhaseLabel.TextColor = isDark ? WorkTextDark : WorkTextLight;
        }
        else
        {
            StatusBadge.BackgroundColor = isDark ? BreakBgDark : BreakBgLight;
            PhaseLabel.TextColor = isDark ? BreakTextDark : BreakTextLight;
        }
    }

    private void PlayPauseButton_Clicked(object sender, EventArgs e)
    {
        if (_timerService.IsRunning)
            _timerService.Pause();
        else
            _timerService.Start();

        UpdateUI();
    }

    private void StopButton_Clicked(object sender, EventArgs e)
    {
        // 提前结束当前阶段
        _timerService.EndPhaseEarly();
        UpdateUI();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timerService.OnTick -= OnTimerTick;
        _timerService.OnPhaseCompleted -= OnPhaseCompleted;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // 防止在页面未完全加载或极小时计算错误
        if (width <= 0 || height <= 0) return;

        // 计算逻辑：
        // 1. 我们希望字体大小与窗口的高度成正比。
        // 2. 根据布局，中间的 Label 大约占据了高度的 50%-60%。
        // 3. 我们也需要考虑宽度，防止字数多了（例如 "60:00"）横向溢出。

        // 这里的系数 0.25 是根据经验调整的：字体大小约为窗口高度的 1/4
        double fontSizeBasedOnHeight = height * 0.25;

        // 这里的系数 0.18 是为了防止宽度过窄时文字切断
        double fontSizeBasedOnWidth = width * 0.18;

        // 取两者中较小的一个，确保既不超高也不超宽
        double optimalFontSize = Math.Min(fontSizeBasedOnHeight, fontSizeBasedOnWidth);

        // 设置上下限，防止字体变得无限小或过大破坏布局
        double clampedFontSize = Math.Clamp(optimalFontSize, 12, 100);

        if (TimeLabel.FontSize != clampedFontSize)
        {
            TimeLabel.FontSize = clampedFontSize;
        }
    }
}