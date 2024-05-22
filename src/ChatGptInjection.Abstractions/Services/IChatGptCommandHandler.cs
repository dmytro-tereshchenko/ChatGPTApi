using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface IChatGptCommandHandler
{
    Task<ChatMessageResponse> ChatComunicationHandleAsync(ChatMessageRequest messageContext);
}
