using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface IHistoryHandler
{
    List<ChatHistory> ChatHistoryHandle();
    MessageHistoryResponse MessageHistoryHandle(string chatId, bool fullResponse);
}
