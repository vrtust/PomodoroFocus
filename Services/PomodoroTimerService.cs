using PomodoroFocus.Models;


namespace PomodoroFocus.Services
{
    public class PomodoroTimerService : IPomodoroTimerService, IDisposable
    {
        private System.Timers.Timer _timer;
        private readonly ISettingsService _settingsService;
        private readonly IWindowActivationService _windowActivationService;

        private PomodoroSession _currentSession;
        private DateTime _phaseStartTime; // 用于计算实际时长
        private TimerCurrentState _currentState = TimerCurrentState.Stopped;

        public TimeSpan TimeLeft { get; private set; }
        public bool IsRunning { get; private set; }
        public PomodoroCycleState CurrentCycleState { get; private set; }

        public event Action OnTick;
        public event Action<PomodoroSession, PomodoroCycleState> OnPhaseCompleted;

        public PomodoroTimerService(ISettingsService settingsService, IWindowActivationService windowActivationService = null)
        {
            // 文档建议：对于需要精确计时的UI应用，使用高分辨率的计时器。
            _timer = new System.Timers.Timer(1000); // 设置计时器间隔为1秒 (1000毫秒)
            _timer.Elapsed += OnTimerElapsed; // 订阅计时器的Elapsed事件
            _settingsService = settingsService;

            _windowActivationService = windowActivationService;

            RestoreState();
        }

        private void RestoreState()
        {
            var settings = _settingsService.CurrentSettings;
            _currentState = settings.TimerState;
            CurrentCycleState = settings.CurrentCycleState;
            _currentSession = settings.ActiveSession;

            if (_currentState == TimerCurrentState.Stopped || _currentSession == null)
            {
                Reset(); // 如果是停止状态或没有活动会话，直接重置
                return;
            }

            var duration = (CurrentCycleState == PomodoroCycleState.Work)
                ? settings.PomodoroDuration
                : settings.ShortBreakDuration;
            var totalDuration = TimeSpan.FromMinutes(duration);

            if (_currentState == TimerCurrentState.Running)
            {
                var elapsed = DateTime.Now - settings.PhaseStartTime;
                if (elapsed < totalDuration)
                {
                    TimeLeft = totalDuration - elapsed;
                    IsRunning = true;
                    _timer.Start();
                }
                else // 应用关闭期间，计时已结束
                {
                    CompletePhase(false);
                }
            }
            else // Paused
            {
                TimeLeft = settings.TimeLeftOnPause;
                IsRunning = false;
            }
            OnTick?.Invoke();
        }

        private async Task PersistStateAsync()
        {
            var settings = _settingsService.CurrentSettings;
            settings.TimerState = _currentState;
            settings.CurrentCycleState = CurrentCycleState;
            settings.ActiveSession = _currentSession;

            if (_currentState == TimerCurrentState.Paused)
            {
                settings.TimeLeftOnPause = TimeLeft;
            }
            if (_currentState == TimerCurrentState.Running)
            {
                // 在Start/BeginNextPhase中设置的_phaseStartTime会被保存
                // 此处我们假设_phaseStartTime是该类的私有成员
                // 但为了持久化，它必须被保存。
                // 更好的做法是在PersistStateAsync中决定要保存什么。
                // 我们在Start中设置它，这里读取并保存。
                var phaseStartTimeField = typeof(PomodoroTimerService).GetField("_phaseStartTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (phaseStartTimeField != null)
                {
                    settings.PhaseStartTime = (DateTime)phaseStartTimeField.GetValue(this);
                }
            }

            await _settingsService.SaveSettingsAsync();
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));

            // 触发我们自己的OnTick事件，通知UI更新
            OnTick?.Invoke();

            if (TimeLeft.TotalSeconds <= 0)
            {
                CompletePhase(false); // isEndedEarly = false
            }
        }

        private async Task CompletePhase(bool isEndedEarly)
        {
            _timer.Stop();
            IsRunning = false;
            _currentState = TimerCurrentState.Stopped;

            var elapsed = DateTime.Now - _phaseStartTime;
            var completedState = this.CurrentCycleState;

            // 记录实际时长
            if (CurrentCycleState == PomodoroCycleState.Work)
            {
                _currentSession.ActualWorkDurationMinutes = (int)Math.Round(elapsed.TotalMinutes);
            }
            else
            {
                _currentSession.ActualBreakDurationMinutes = (int)Math.Round(elapsed.TotalMinutes);
            }

            _windowActivationService?.ActivateMainWindow();

            await PersistStateAsync();
            // 触发阶段完成事件
            OnPhaseCompleted?.Invoke(_currentSession, completedState);
        }

        public async void Start()
        {
            if (IsRunning) return;

            // 如果状态是 Stopped，说明要开始一个全新的会话
            if (_currentState == TimerCurrentState.Stopped)
            {
                _currentSession = new PomodoroSession
                {
                    StartTime = DateTime.Now,
                    PlannedWorkDurationMinutes = _settingsService.CurrentSettings.PomodoroDuration,
                    PlannedBreakDurationMinutes = _settingsService.CurrentSettings.ShortBreakDuration
                };
                CurrentCycleState = PomodoroCycleState.Work;
                TimeLeft = TimeSpan.FromMinutes(_currentSession.PlannedWorkDurationMinutes);
            }

            // 对于 Paused 和新开始的 Stopped 状态，都执行以下恢复/启动逻辑
            _phaseStartTime = DateTime.Now - (TimeSpan.FromMinutes(GetPhaseDuration()) - TimeLeft); // 校准开始时间
            IsRunning = true;
            _currentState = TimerCurrentState.Running;
            _timer.Start();
            await PersistStateAsync();
            OnTick?.Invoke();
        }

        public async void BeginNextPhase()
        {
            if (IsRunning) return;

            // 明确地将状态翻转到下一阶段
            CurrentCycleState = (CurrentCycleState == PomodoroCycleState.Work)
                ? PomodoroCycleState.ShortBreak
                : PomodoroCycleState.Work;

            var duration = (CurrentCycleState == PomodoroCycleState.Work)
                ? _currentSession.PlannedWorkDurationMinutes
                : _currentSession.PlannedBreakDurationMinutes;

            TimeLeft = TimeSpan.FromMinutes(duration);
            _phaseStartTime = DateTime.Now;
            IsRunning = true;
            _timer.Start();
            await PersistStateAsync();
            OnTick?.Invoke();
        }

        public async void Pause()
        {
            if (!IsRunning) return;

            _timer.Stop();
            IsRunning = false;
            _currentState = TimerCurrentState.Paused; // 明确设置状态为暂停
            await PersistStateAsync();
            OnTick?.Invoke();
        }

        public async void Reset()
        {
            _timer.Stop();
            IsRunning = false;
            _currentState = TimerCurrentState.Stopped; // 明确设置状态为停止
            CurrentCycleState = PomodoroCycleState.Work;
            TimeLeft = TimeSpan.FromMinutes(_settingsService.CurrentSettings.PomodoroDuration);
            _currentSession = null;
            await PersistStateAsync();
            OnTick?.Invoke();
        }

        public async void EndPhaseEarly()
        {
            if (!IsRunning) return;
            await CompletePhase(true); // isEndedEarly = true
        }

        private int GetPhaseDuration()
        {
            return (CurrentCycleState == PomodoroCycleState.Work)
                ? _currentSession.PlannedWorkDurationMinutes
                : _currentSession.PlannedBreakDurationMinutes;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
