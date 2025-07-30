using PomodoroFocus.Services;
using PomodoroFocus.Models;

namespace PomodoroFocus;

public partial class WidgetPage : ContentPage
{
    private readonly IPomodoroTimerService _timerService;
    public WidgetPage(IPomodoroTimerService timerService, IFloatingWindowService floatingWindowService)
    {
        InitializeComponent();
        _timerService = timerService;
        _timerService.OnTick += OnTimerTick;
        // 初始化UI
        OnTimerTick();
    }

    private void OnTimerTick()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimeLabel.Text = _timerService.TimeLeft.ToString("mm\\:ss");
            PhaseLabel.Text = _timerService.CurrentCycleState == PomodoroCycleState.Work ? "工作" : "休息";
            PlayPauseButton.Text = _timerService.IsRunning ? "暂停" : "开始";
        });
    }

    private void PlayPauseButton_Clicked(object sender, EventArgs e)
    {
        if (_timerService.IsRunning)
        {
            _timerService.Pause();
        }
        else
        {
            _timerService.Start();
        }
    }

    // 确保在页面关闭时取消订阅
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timerService.OnTick -= OnTimerTick;
    }
}