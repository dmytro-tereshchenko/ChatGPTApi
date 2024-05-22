namespace ChatGptInjection.Abstractions.Models;

public class MessageHistoryResponse
{
    public string ChatId { get; set; }
    public List<MessageHistory> MessageHistory { get; set; }

    public MessageHistoryResponse()
    {
        ChatId = string.Empty;
        MessageHistory = new List<MessageHistory>();
    }
}

public record MessageHistory(string RequestMessage, string Model, DateTime CreatedUtcTime, string ResponseMessage, OpenAiChatResponse? response);
