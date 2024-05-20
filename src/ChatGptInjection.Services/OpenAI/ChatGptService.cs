using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace ChatGptInjection.Services.OpenAI;

public class ChatGptService : IChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IConfigService _configService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<ChatGptService> _logger;

    public ChatGptService(IHttpClientFactory httpClientFactory, 
        IBlobStorageService blobStorageService,
        IConfigService configService,
        IOptions<AppSettings> appSettingsOptions,
        ILogger<ChatGptService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("OpenAI");
        _blobStorageService = blobStorageService;
        _configService = configService;
        _appSettings = appSettingsOptions.Value;
        _logger = logger;
    }

    public async Task<ChatMessageResponse> SendMessage(ChatMessageRequest messageContext)
    {
        if (string.IsNullOrEmpty(messageContext.ChatId))
            messageContext.ChatId = Guid.NewGuid().ToString();

        OpenAiChatRequestBody requestBody = new() { Model = GetDescription(messageContext.Model) };
        requestBody.Messages.Add(new ChatMessage("user", messageContext.Message));

        var content = BuildContent(requestBody);

        var openAiApiKey = _configService.GetOpenAiApiKey(messageContext.Model);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);

        var url = _appSettings.ChatCompletionEndpoint;

        OpenAiChatResponse chatResponse;

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            if(!response.IsSuccessStatusCode) throw new Exception($"Bad request to OpenAI API, Status Code {response.StatusCode}");
            var stringContent = await response.Content.ReadAsStringAsync();
            chatResponse = JsonConvert.DeserializeObject<OpenAiChatResponse>(stringContent)!;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while http request to OpenAI API: {@ex}", ex);
            throw;
        }

        chatResponse.ChatId = messageContext.ChatId;

        await _blobStorageService.UploadMessage(chatResponse);

        return new ChatMessageResponse() { 
            ChatId = messageContext.ChatId,
            Message = chatResponse.Choices.First().Message.Content
        };
    }

    private string GetDescription(OpenAiModels model)
    {
        FieldInfo? fieldInfo = model.GetType().GetField(model.ToString());
        if (fieldInfo is null) return string.Empty;
        var attribute = (DescriptionAttribute?)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
        return attribute?.Description ?? string.Empty;
    }

    private StringContent BuildContent(OpenAiChatRequestBody requestBody)
    {
        var serializerSettings = new JsonSerializerSettings();
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        var requestBodyString = JsonConvert.SerializeObject(requestBody, serializerSettings);
        var content = new StringContent(requestBodyString, Encoding.UTF8);

        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return content;
    }
}
