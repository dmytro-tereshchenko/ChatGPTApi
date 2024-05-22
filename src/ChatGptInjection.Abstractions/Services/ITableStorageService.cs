using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface ITableStorageService
{
    Task StoreRequestMessage(StorageMessageEntity requestMessage);
    List<StorageMessageEntity> GetChatList();
    List<StorageMessageEntity> GetMessageHistory(string chatId);
}
