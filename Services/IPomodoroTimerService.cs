using PomodoroFocus.Models;

namespace PomodoroFocus.Services
{
    public interface IPomodoroTimerService
    {
        TimeSpan TimeLeft { get; }
        bool IsRunning { get; }
        PomodoroCycleState CurrentCycleState { get; }
        bool IsLogModalPending { get; }

        event Action OnTick;
        event Action<PomodoroSession, PomodoroCycleState> OnPhaseCompleted;
        event Action OnLogModalStateChanged;

        void Start();
        void Pause();
        void Reset();
        void EndPhaseEarly();
        void BeginNextPhase();
        void AcknowledgeLogModal();
    }
}
