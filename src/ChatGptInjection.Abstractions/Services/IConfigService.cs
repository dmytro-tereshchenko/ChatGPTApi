namespace ChatGptInjection.Abstractions.Services;

public interface IConfigService
{
    string GetAzureConnectionString();
    string GetOpenAiApiKey(OpenAiModels model);
}
