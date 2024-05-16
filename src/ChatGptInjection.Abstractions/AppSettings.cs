using System.ComponentModel;

namespace AppSec.AIPromtInjection.Abstractions;

#pragma warning disable CS8618
public class AppSettings
{
    public string OpenAiHost { get; set; }
    public string ChatCompletionEndpoint { get; set; }
    public string ChatGpt35ApiKeyName { get; set; }
    public string ChatGpt4ApiKeyName { get; set; }
}

public enum OpenAiModels
{
    [Description("gpt-3.5-turbo")]
    Gpt35Turbo,

    [Description("gpt-4-turbo")]
    Gpt4Turbo
}
