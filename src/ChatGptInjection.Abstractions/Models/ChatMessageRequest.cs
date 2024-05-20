namespace ChatGptInjection.Abstractions.Models;

public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public OpenAiModels Model { get; set; }
    public string ChatId { get; set; } = string.Empty;
}
