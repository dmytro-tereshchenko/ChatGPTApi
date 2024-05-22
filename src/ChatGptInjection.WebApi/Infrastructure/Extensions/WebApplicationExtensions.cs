using ChatGptInjection.WebApi.Infrastructure.Constants;
using ChatGptInjection.WebApi.Infrastructure.EndPoints;
using ChatGptInjection.WebApi.Infrastructure.Middlewares;

namespace ChatGptInjection.WebApi.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseChatContentValidationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ChatContentValidationMiddleware>();
    }

    public static void RegisterRoutes(this WebApplication app)
    {
        app.MapGet("/", () => "Test AI promt injection WebApi");
        app.MapPost(Routing.SEND_MESSAGE, EndPointsGroup.SendMessage);
        app.MapGet(Routing.GET_CHAT_HISTORY, EndPointsGroup.GetChatHistory);
        app.MapGet(Routing.GET_MESSAGE_HISTORY, EndPointsGroup.GetMessageHistory);
        app.MapGet($"{Routing.GET_MESSAGE_HISTORY}/{{chatId}}", EndPointsGroup.GetMessageHistory);
    }
}
