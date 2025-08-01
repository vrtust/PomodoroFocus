namespace PomodoroFocus.Models
{
    public enum TimerCurrentState { Stopped, Running, Paused }
    public enum PomodoroCycleState { Work, ShortBreak }

    public class AppSettings
    {
        public int PomodoroDuration { get; set; } = 25;
        public int ShortBreakDuration { get; set; } = 5;
        public string Theme { get; set; } = "System"; // "System", "Light", "Dark"
        public string Language { get; set; } = "en-US"; // "en-US", "zh-CN"

        public string ApiKey { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty; // For custom endpoints

        public TimerCurrentState TimerState { get; set; } = TimerCurrentState.Stopped;
        public PomodoroCycleState CurrentCycleState { get; set; } = PomodoroCycleState.Work;
        public PomodoroSession? ActiveSession { get; set; }
        public DateTime PhaseStartTime { get; set; }
        public TimeSpan TimeLeftOnPause { get; set; }
    }
}
