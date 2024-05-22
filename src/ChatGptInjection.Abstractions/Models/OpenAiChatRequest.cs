namespace ChatGptInjection.Abstractions.Models;

public class OpenAiChatRequest
{
    public string Model { get; set; }
    public List<ChatMessage> Messages { get; set; }

    public OpenAiChatRequest()
    {
        Model = string.Empty;
        Messages = new List<ChatMessage>();
    }
}

public record ChatMessage(string Role, string Content);
