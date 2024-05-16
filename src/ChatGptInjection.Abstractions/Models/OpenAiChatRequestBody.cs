namespace AppSec.AIPromtInjection.Abstractions.Models;

#pragma warning disable CS8618
public class OpenAiChatRequestBody
{
    public string Model { get; set; }
    public List<ChatMessage> Messages { get; set; }

    public OpenAiChatRequestBody() => Messages = new List<ChatMessage>();
}

public record ChatMessage(string Role, string Content);
