using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using Azure.Messaging;
using Newtonsoft.Json;

namespace ChatGptInjection.Services.Handlers;

public class HistoryHandler : IHistoryHandler
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ITableStorageService _tableStorageService;

    public HistoryHandler(IBlobStorageService blobStorageService, ITableStorageService tableStorageService)
    {
        _blobStorageService = blobStorageService;
        _tableStorageService = tableStorageService;
    }

    public List<ChatHistory> ChatHistoryHandle()
    {
        var storageChats = _tableStorageService.GetChatList();
        var chatList = storageChats.Select(ent => new ChatHistory()
        {
            ChatId = ent.ChatId,
            Model = ent.Model,
            UtcCreationTime = DateTimeOffset.FromUnixTimeSeconds(ent.Created).DateTime,
        }).ToList();
        return chatList;
    }

    public MessageHistoryResponse MessageHistoryHandle(string chatId, bool fullResponse)
    {
        var messages = _tableStorageService.GetMessageHistory(chatId);
        var response = new MessageHistoryResponse()
        {
            ChatId = chatId,
            MessageHistory = messages.Select(async mes => await GetMessageHistory(mes, fullResponse))
                                    .Select(t => t.Result)
                                    .Where(i => i != null)
                                    .OrderBy(e => e.CreatedUtcTime)
                                    .ToList()
        };
        return response;
    }

    private async Task<MessageHistory> GetMessageHistory(StorageMessageEntity message, bool fullResponse)
    {
        var responseMessage = await _blobStorageService.DownloadMessage(message.ResponseStorageId);
        var createdTime = DateTimeOffset.FromUnixTimeSeconds(message.Created).DateTime;
        var response = fullResponse ? new MessageHistory(message.RequestMessage,
            message.Model,
            createdTime,
            string.Empty,
            responseMessage!) :
            new MessageHistory(message.RequestMessage,
            message.Model,
            createdTime,
            responseMessage?.Choices.FirstOrDefault()?.Message.Content ?? "", default);
        
        return response;
    }
}
