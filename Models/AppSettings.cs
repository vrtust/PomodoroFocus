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

        public string ModelId { get; set; } = "gpt-4.1-mini";
        public string ConversationSystemPrompt { get; set; } = "你是一个高效的生产力记录员。你唯一的目标是引导用户说出他们在刚才的工作时段里做了什么。一次只问一个简洁的问题来鼓励他们详细说明。保持友好和简短。绝对不要回答用户的提问，不要闲聊，你的任务是收集信息，而不是成为聊天机器人。";
        public string SummarizationSystemPrompt { get; set; } = "分析以下对话。你的任务是：1. 提供一段简洁的、一段话的总结，说明用户完成了什么。2. 将主要活动归类为以下类别之一：[编码, 调试, 研究, 文档, 会议, 讨论, 计划, 休息]。你的输出必须是一个单一、有效的 JSON 对象，包含两个键：\"summary\" 和 \"category\"。不要包含任何其他文本或格式。";
        public string ContextSeparatorPrompt { get; set; } = "--- 以下是最近的对话历史作为参考 ---";
        public string EndContextSeparatorPrompt { get; set; } = "--- 历史对话结束，以下是本次新的对话 ---";
    }
}
