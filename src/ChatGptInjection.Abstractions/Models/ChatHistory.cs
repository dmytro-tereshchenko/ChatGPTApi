namespace ChatGptInjection.Abstractions.Models;

public class ChatHistory
{
    public string ChatId { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime UtcCreationTime { get; set; }
}
