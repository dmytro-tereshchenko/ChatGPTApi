using ChatGptInjection.Abstractions.Services;
using ChatGptInjection.Abstractions;
using Microsoft.Extensions.Options;
using ChatGptInjection.Abstractions.Models;
using SharpToken;

namespace ChatGptInjection.Services.Handlers;

public class ChatGptCommandHandler : IChatGptCommandHandler
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ITableStorageService _tableStorageService;
    private readonly IChatGptService _chatGptService;
    private readonly AppSettings _appSettings;

    public ChatGptCommandHandler(IBlobStorageService blobStorageService,
        ITableStorageService tableStorageService,
        IChatGptService chatGptService,
        IOptions<AppSettings> appSettingsOptions)
    {
        _blobStorageService = blobStorageService;
        _tableStorageService = tableStorageService;
        _chatGptService = chatGptService;
        _appSettings = appSettingsOptions.Value;
    }

    public async Task<ChatMessageResponse> ChatComunicationHandleAsync(ChatMessageRequest messageContext)
    {
        var messagesList = new List<ChatMessage>();

        if (string.IsNullOrEmpty(messageContext.ChatId))
            messageContext.ChatId = Guid.NewGuid().ToString();
        else
            messagesList = await GetChatHistory(messageContext);

        messagesList.Add(new ChatMessage("user", messageContext.Message));

        var chatResponse = await _chatGptService.SendMessageAsync(messageContext.Model, messagesList);

        chatResponse.ChatId = messageContext.ChatId;

        var blobName = await _blobStorageService.UploadMessage(chatResponse);

        var storeRequestMessage = new StorageMessageEntity()
        {
            ChatId = chatResponse.ChatId,
            RequestMessage = messageContext.Message,
            Model = _appSettings.ChatGptModels[messageContext.Model]?.Model ?? "",
            ResponseStorageId = blobName
        };

        await _tableStorageService.StoreRequestMessage(storeRequestMessage);

        return new ChatMessageResponse()
        {
            ChatId = messageContext.ChatId,
            Message = chatResponse.Choices.First().Message.Content
        };
    }

    private int CalculateTokens(string message, string model)
    {
        var chatModel = _appSettings.ChatGptModels[model];
        var encodingName = Model.GetEncodingNameForModel(chatModel.Model) ?? chatModel.Encoding;
        var modelEncoding = GptEncoding.GetEncoding(encodingName);
        var encoded = modelEncoding.Encode($"{{ role: user, content: {message} }}");
        return encoded.Count();
    }

    private async Task<List<ChatMessage>> GetChatHistory(ChatMessageRequest messageContext)
    {
        var chatHistory = new List<ChatMessage>();
        var messages = _tableStorageService.GetMessageHistory(messageContext.ChatId)
                .OrderByDescending(mes => mes.Created);

        var maxTokens = _appSettings.MaxTokensOnRequest;
        var tokens = CalculateTokens(messageContext.Message, messageContext.Model);

        foreach (var message in messages)
        {
            var responseMessage = await _blobStorageService.DownloadMessage(message.ResponseStorageId);

            if (responseMessage is null)
                continue;

            var requestMessageTokens = CalculateTokens(message.RequestMessage, messageContext.Model);

            if (tokens + requestMessageTokens + responseMessage.Usage!.Completion_Tokens < maxTokens)
            {
                chatHistory.Add(new ChatMessage("assistant", responseMessage.Choices.FirstOrDefault()?.Message.Content ?? ""));
                chatHistory.Add(new ChatMessage("user", message.RequestMessage));
                tokens += requestMessageTokens + responseMessage.Usage!.Completion_Tokens;
            }
            else
                break;
        }

        chatHistory.Reverse();

        return chatHistory;
    }
}
