namespace AppSec.AIPromtInjection.Abstractions.Models;

#pragma warning disable CS8618
public class ChatRequestDto
{
    public string Message { get; set; }
    public OpenAiModels Model { get; set; }
    public string ChatId { get; set; }
}
