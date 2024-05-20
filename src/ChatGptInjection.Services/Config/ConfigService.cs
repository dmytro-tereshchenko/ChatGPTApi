using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatGptInjection.Services.Config;

public class ConfigService : IConfigService
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<ConfigService> _logger;

    public ConfigService(IOptions<AppSettings> appSettingsOptions,
        ILogger<ConfigService> logger)
    {
        _appSettings = appSettingsOptions.Value;
        _logger = logger;
    }

    public string GetAzureConnectionString()
    {
        var azureConnectionString =  Environment.GetEnvironmentVariable(_appSettings.AzureConnectionStringName);

        if (azureConnectionString is null)
            LogError("Azure connection string to Storage account is not found");

        return azureConnectionString!;
    }

    public string GetOpenAiApiKey(OpenAiModels model)
    {
        string? openAiApiKey = model switch
        {
            OpenAiModels.Gpt4Turbo => Environment.GetEnvironmentVariable(_appSettings.ChatGpt4ApiKeyName),
            OpenAiModels.Gpt35Turbo or _ => Environment.GetEnvironmentVariable(_appSettings.ChatGpt35ApiKeyName)
        };

        if (openAiApiKey is null)
            LogError($"OpenAI Api key is not found for {model}");

        return openAiApiKey!;
    }

    private void LogError(string message)
    {
        _logger.LogError(message);
        throw new Exception(message);
    }
}
