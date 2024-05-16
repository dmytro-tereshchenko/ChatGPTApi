using AppSec.AIPromtInjection.Abstractions;
using AppSec.AIPromtInjection.Abstractions.Models;
using AppSec.AIPromtInjection.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace AppSec.AIPromtInjection.Services.OpenAI;

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

    public async Task<string> SendMessage(ChatRequestDto messageContext)
    {
        OpenAiChatRequestBody requestBody = new() { Model = GetDescription(messageContext.Model) };
        requestBody.Messages.Add(new ChatMessage("user", messageContext.Message));

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

        var url = _appSettings.ChatCompletionEndpoint;
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8);
        content.Headers.Add("Authorization", $"Bearer {openAiApiKey}");

        OpenAiChatResponse chatResponse;

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var stringContent = await response.Content.ReadAsStringAsync();
            chatResponse = JsonConvert.DeserializeObject<OpenAiChatResponse>(stringContent)!;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while http request to OpenAi API", ex);
            throw;
        }

        return chatResponse.Choices.First().Message.Content;
    }

    private string GetDescription(OpenAiModels model)
    {
        FieldInfo? fieldInfo = model.GetType().GetField(model.ToString());
        if (fieldInfo is null) return string.Empty;
        var attribute = (DescriptionAttribute?)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
        return attribute?.Description ?? string.Empty;
    }
}
