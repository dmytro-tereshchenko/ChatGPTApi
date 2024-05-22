using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Text;

namespace ChatGptInjection.Services.OpenAI;

public class ChatGptService : IChatGptService
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<ChatGptService> _logger;

    public ChatGptService(IOptions<AppSettings> appSettingsOptions,
        ILogger<ChatGptService> logger)
    {
        _appSettings = appSettingsOptions.Value;
        _logger = logger;
    }

    public async Task<OpenAiChatResponse> SendMessageAsync(string model, List<ChatMessage> messages)
    {
        var chatGptModel = _appSettings.ChatGptModels[model];

        OpenAiChatResponse chatResponse;

        try
        {
            OpenAiChatRequest requestBody = new() { Model = chatGptModel.Model };
            requestBody.Messages.AddRange(messages);

            var content = BuildContent(requestBody);

            var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", chatGptModel.ApiKey);

            var response = await _httpClient.PostAsync(chatGptModel.Endpoint, content);

            if(!response.IsSuccessStatusCode) throw new Exception($"Bad request to OpenAI API, Status Code {response.StatusCode}");
            
            var stringContent = await response.Content.ReadAsStringAsync();
            chatResponse = JsonConvert.DeserializeObject<OpenAiChatResponse>(stringContent)!;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while http request to OpenAI API: {@ex}", ex);
            throw;
        }

        return chatResponse;
    }

    private StringContent BuildContent(OpenAiChatRequest requestBody)
    {
        var serializerSettings = new JsonSerializerSettings();
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        var requestBodyString = JsonConvert.SerializeObject(requestBody, serializerSettings);
        var content = new StringContent(requestBodyString, Encoding.UTF8);

        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return content;
    }
}
