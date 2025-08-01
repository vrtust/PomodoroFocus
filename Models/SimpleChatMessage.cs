namespace PomodoroFocus.Models
{
    public class SimpleChatMessage
    {
        // "User", "Assistant", "System"等
        public string Role { get; set; }

        // 消息的文本内容
        public string Content { get; set; }
    }
}
