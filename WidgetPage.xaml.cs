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
        // ��ʼ��UI
        OnTimerTick();
    }

    private void OnTimerTick()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimeLabel.Text = _timerService.TimeLeft.ToString("mm\\:ss");
            PhaseLabel.Text = _timerService.CurrentCycleState == PomodoroCycleState.Work ? "����" : "��Ϣ";
            PlayPauseButton.Text = _timerService.IsRunning ? "��ͣ" : "��ʼ";
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

    // ȷ����ҳ��ر�ʱȡ������
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timerService.OnTick -= OnTimerTick;
    }
}