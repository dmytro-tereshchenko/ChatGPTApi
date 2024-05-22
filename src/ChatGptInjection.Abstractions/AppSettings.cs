namespace ChatGptInjection.Abstractions;

public class AppSettings
{
    public string AzureConnectionString { get; set; } = string.Empty;
    public string ChatGptResponsesBlobContainer { get; set; } = string.Empty;
    public string ChatGptStorageHistoryContainer { get; set; } = string.Empty;
    public int MaxTokensOnRequest { get; set; }
    public Dictionary<string, ChatGptModel> ChatGptModels { get; set; } = new Dictionary<string, ChatGptModel>();
}

public record ChatGptModel(string Endpoint, string Model, string ApiKey, string Encoding);
