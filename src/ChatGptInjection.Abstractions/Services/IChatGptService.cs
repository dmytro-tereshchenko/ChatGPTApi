using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface IChatGptService
{
    Task<OpenAiChatResponse> SendMessageAsync(string model, List<ChatMessage> messages);
}
