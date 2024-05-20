namespace ChatGptInjection.Abstractions.Models;

public class OpenAiChatRequestBody
{
    public string Model { get; set; }
    public List<ChatMessage> Messages { get; set; }

    public OpenAiChatRequestBody()
    {
        Model = string.Empty;
        Messages = new List<ChatMessage>();
    }
}

public record ChatMessage(string Role, string Content);
