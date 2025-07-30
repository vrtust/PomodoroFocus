using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string UserInput { get; set; } // 对话历史JSON
        public string LLMSummary { get; set; }
        public string ActivityCategory { get; set; }
    }
}
