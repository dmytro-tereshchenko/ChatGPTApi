using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface IChatGptService
{
    Task<ChatResponseDto> SendMessage(ChatRequestDto messageContext);
}
