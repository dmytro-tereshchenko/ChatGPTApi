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
        app.MapGet(Routing.GET_HISTORY, EndPointsGroup.GetHistory);
        app.MapGet($"{Routing.GET_HISTORY}/{{chatId}}", EndPointsGroup.GetHistory);
    }
}
