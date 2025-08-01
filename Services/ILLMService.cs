using OpenAI.Chat;

namespace PomodoroFocus.Services
{
    public interface ILLMService
    {
        Task<string> GetInitialPromptAsync();
        Task<string> GetNextResponseAsync(List<ChatMessage> history);
        Task<(string Summary, string Category)> SummarizeConversationAsync(List<ChatMessage> history);
    }
}
