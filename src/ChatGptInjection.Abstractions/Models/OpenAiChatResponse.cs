namespace AppSec.AIPromtInjection.Abstractions.Models;

#pragma warning disable CS8618
public class OpenAiChatResponse
{
    public string Id { get; set; }
    public string ChatId { get; set; }
    public long Created { get; set; }
    public string Model { get; set; }
    public string System_Fingeprint { get; set; }
    public List<ChatChoice> Choices { get; set; }
    public Usage Usage { get; set; }

    public OpenAiChatResponse() => Choices = new List<ChatChoice>();
}

public record ChatChoice(int Index, ChatMessage Message, string? Logprobs, string Finish_Reason);

public record Usage(int Prompt_Tokens, int Completion_Tokens, int Total_Tokens);
