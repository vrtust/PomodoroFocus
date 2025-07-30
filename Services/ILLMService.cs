using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroFocus.Services
{
    public interface ILLMService
    {
        Task<string> GetInitialPromptAsync();
        Task<string> GetNextResponseAsync(List<ChatMessage> history);
        Task<(string Summary, string Category)> SummarizeConversationAsync(List<ChatMessage> history);
    }
}
