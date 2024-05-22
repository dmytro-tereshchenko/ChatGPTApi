namespace ChatGptInjection.Abstractions.Models;

public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
}
