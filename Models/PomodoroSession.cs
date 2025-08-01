namespace PomodoroFocus.Models
{
    public class PomodoroSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PlannedWorkDurationMinutes { get; set; } // 计划的工作时长
        public int ActualWorkDurationMinutes { get; set; }  // 实际的工作时长
        public int PlannedBreakDurationMinutes { get; set; } // 计划的休息时长
        public int ActualBreakDurationMinutes { get; set; } // 实际的休息时长

        public string UserInput { get; set; } = string.Empty;  // 对话历史JSON
        public string LLMSummary { get; set; } = string.Empty;
        public string ActivityCategory { get; set; } = string.Empty;
    }
}
