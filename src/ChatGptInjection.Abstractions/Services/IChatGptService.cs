using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface IChatGptService
{
    Task<ChatMessageResponse> SendMessage(ChatMessageRequest messageContext);
}
