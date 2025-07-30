using OpenAI;
using OpenAI.Chat;
using PomodoroFocus.Models;
using System.ClientModel;
using System.Text.Json;

namespace PomodoroFocus.Services;

public class LLMService : ILLMService
{
    private readonly ISettingsService _settingsService;
    private ChatClient _client;

    public LLMService(ISettingsService settingsService) => _settingsService = settingsService;
    public Task<string> GetInitialPromptAsync()
    {
        // This can be a static prompt and doesn't need an API call.
        // It's the first thing the assistant says to start the conversation.
        return Task.FromResult("恭喜你完成一个番茄钟，这次在做什么？");
    }

    // 每次调用前都初始化客户端，以确保使用的是最新的用户设置
    private void InitializeClient()
    {
        var settings = _settingsService.CurrentSettings;
        if (string.IsNullOrEmpty(settings.ApiKey))
        {
            // 如果用户没有在设置里提供 Key，则直接抛出异常
            throw new InvalidOperationException("API Key 未在设置中提供。");
        }

        var clientOptions = new OpenAIClientOptions();
        // 如果用户提供了自定义的 Base URL，就使用它
        if (!string.IsNullOrEmpty(settings.ApiBaseUrl) && Uri.TryCreate(settings.ApiBaseUrl, UriKind.Absolute, out var endpoint))
        {
            clientOptions.Endpoint = endpoint;
        }

        // 注意这里的参数，model 可以将来也做成可配置
        _client = new ChatClient(
            model: "gpt-4.1-mini",
            credential: new ApiKeyCredential(settings.ApiKey),
            options: clientOptions
        );
    }

    public async Task<string> GetNextResponseAsync(List<ChatMessage> history)
    {
        InitializeClient();

        // 插入我们的系统指令，来限制LLM的行为
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("你是一个高效的生产力记录员。你唯一的目标是引导用户说出他们在刚才的工作时段里做了什么。一次只问一个简洁的问题来鼓励他们详细说明。保持友好和简短。绝对不要回答用户的提问，不要闲聊，你的任务是收集信息，而不是成为聊天机器人。")
        };
        messages.AddRange(history); // 添加真实对话历史

        var options = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = 1000 // 限制回复长度
        };

        try
        {
            ChatCompletion completion = await _client.CompleteChatAsync(messages, options);
            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            // 记录日志或者其他错误处理，此处可根据需要调整
            throw new Exception("获取响应时出错：" + ex.Message, ex);
        }
    }

    public async Task<(string Summary, string Category)> SummarizeConversationAsync(List<ChatMessage> history)
    {
        if(history.Count == 1)
        {
            return ("在该番茄钟完成后并未进行任何对话", "");
        }
        InitializeClient();

        // 插入用于总结的、不同的系统指令
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("分析以下对话。你的任务是：1. 提供一段简洁的、一段话的总结，说明用户完成了什么。2. 将主要活动归类为以下类别之一：[编码, 调试, 研究, 文档, 会议, 讨论, 计划, 休息]。你的输出必须是一个单一、有效的 JSON 对象，包含两个键：\"summary\" 和 \"category\"。不要包含任何其他文本或格式。"),
        };
        messages.AddRange(history);

        var options = new ChatCompletionOptions()
        {
            // 这是官方文档中强制输出 JSON 的正确方法
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        ChatCompletion completion = await _client.CompleteChatAsync(messages, options);
        var jsonResponse = completion.Content[0].Text;

        // 解析返回的JSON
        using var doc = JsonDocument.Parse(jsonResponse);
        var root = doc.RootElement;
        var summary = root.GetProperty("summary").GetString();
        var category = root.GetProperty("category").GetString();

        return (summary, category);
    }
}
