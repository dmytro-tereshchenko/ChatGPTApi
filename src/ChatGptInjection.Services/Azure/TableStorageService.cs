using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace ChatGptInjection.Services.Azure;

public class TableStorageService : ITableStorageService
{
    private readonly AppSettings _appSettings;

    public TableStorageService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
    }

    public async Task StoreRequestMessage(StorageMessageEntity requestMessage)
    {
        var tableClient = new TableClient(_appSettings.AzureConnectionString, _appSettings.ChatGptStorageHistoryContainer);
        var partitionKey = requestMessage.ChatId;
        var rowKey = Guid.NewGuid().ToString();
        var unixCreatedTimestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        var tableEntity = new TableEntity(partitionKey, rowKey)
        {
            { "RequestMessage", requestMessage.RequestMessage },
            { "Model", requestMessage.Model },
            { "ChatId", requestMessage.ChatId },
            { "Created", unixCreatedTimestamp },
            { "ResponseStorageId", requestMessage.ResponseStorageId }
        };

        await tableClient.AddEntityAsync(tableEntity);
    }

    public List<StorageMessageEntity> GetChatList()
    {
        var tableClient = new TableClient(_appSettings.AzureConnectionString, _appSettings.ChatGptStorageHistoryContainer);

        var chats = tableClient.Query<StorageMessageEntity>()
            .OrderBy(ent => ent.ChatId)
            .ThenBy(ent => ent.Created)
            .DistinctBy(ent => ent.ChatId)
            .ToList();
        
        return chats;
    }

    public List<StorageMessageEntity> GetMessageHistory(string chatId)
    {
        var tableClient = new TableClient(_appSettings.AzureConnectionString, _appSettings.ChatGptStorageHistoryContainer);

        var storageMessages = tableClient.Query<StorageMessageEntity>(ent => ent.PartitionKey == chatId)
            .OrderBy(ent => ent.Created)
            .ToList();

        return storageMessages;
    }
}
