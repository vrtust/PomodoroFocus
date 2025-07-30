using PomodoroFocus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroFocus.Services
{
    public interface IPomodoroTimerService
    {
        TimeSpan TimeLeft { get; }
        bool IsRunning { get; }
        PomodoroCycleState CurrentCycleState { get; }

        event Action OnTick;
        event Action<PomodoroSession, PomodoroCycleState> OnPhaseCompleted;

        void Start();
        void Pause();
        void Reset();
        void EndPhaseEarly();
        void BeginNextPhase();
    }
}
