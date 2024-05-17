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
    private readonly AppSettings _appSettings;
    private readonly ILogger<ChatGptService> _logger;

    public ChatGptService(IHttpClientFactory httpClientFactory, 
        IOptions<AppSettings> appSettingsOptions,
        ILogger<ChatGptService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("OpenAI");
        _appSettings = appSettingsOptions.Value;
        _logger = logger;
    }

    public async Task<ChatResponseDto> SendMessage(ChatRequestDto messageContext)
    {
        OpenAiChatRequestBody requestBody = new() { Model = GetDescription(messageContext.Model) };
        requestBody.Messages.Add(new ChatMessage("user", messageContext.Message));

        var content = BuildContent(requestBody);

        string? openAiApiKey = messageContext.Model switch
        {
            OpenAiModels.Gpt4Turbo => Environment.GetEnvironmentVariable(_appSettings.ChatGpt4ApiKeyName),
            OpenAiModels.Gpt35Turbo or _ => Environment.GetEnvironmentVariable(_appSettings.ChatGpt35ApiKeyName)
        };

        if (openAiApiKey is null) {
            var errorMessage = $"OpenAI Api key is not found for {requestBody.Model}";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

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
            _logger.LogError("Error while http request to OpenAI API", ex);
            throw;
        }

        return new ChatResponseDto() { 
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
