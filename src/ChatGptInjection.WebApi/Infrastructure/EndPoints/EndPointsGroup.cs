using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;

namespace ChatGptInjection.WebApi.Infrastructure.EndPoints;

public static class EndPointsGroup
{
    public static async Task<IResult> SendMessage(ChatMessageRequest bodyRequest, IChatGptCommandHandler chatGptHandler)
    {
        try
        {
            if (string.IsNullOrEmpty(bodyRequest.Message))
                return TypedResults.BadRequest();

            var chatResponse = await chatGptHandler.ChatComunicationHandleAsync(bodyRequest);

            return TypedResults.Ok(chatResponse);
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
    }

    public static IResult GetChatHistory(IHistoryHandler historyHandler)
    {
        try
        {
            var chatHistory = historyHandler.ChatHistoryHandle();
            return TypedResults.Ok(chatHistory);
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
    }

    public static IResult GetMessageHistory(string chatId, IHistoryHandler historyHandler, bool fullResponse = false)
    {
        try
        {
            if (string.IsNullOrEmpty(chatId))
                return TypedResults.BadRequest();

            var messageHistory = historyHandler.MessageHistoryHandle(chatId, fullResponse);
            return TypedResults.Ok(messageHistory);
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
    }
}
