using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using ChatGptInjection.WebApi.Infrastructure.Constants;
using ChatGptInjection.WebApi.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace ChatGptInjection.WebApi.Infrastructure.Middlewares;

public class ChatContentValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ChatContentValidationMiddleware> _logger;

    public ChatContentValidationMiddleware(RequestDelegate next, ILogger<ChatContentValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IChatContentValidator validator)
    {
        var useChatConversationEndpoint = context.Request.Path
            .StartsWithSegments(Routing.SEND_MESSAGE, StringComparison.OrdinalIgnoreCase);

        if (useChatConversationEndpoint)
        {
            var requestMessageContent = await GetRequestMessageContent(context.Request);
            var chatRequest = JsonConvert.DeserializeObject<ChatRequestDto>(requestMessageContent);
            var requestValidation = chatRequest is not null ? validator.ChatRequestValidate(chatRequest.Message!) : false;
            if (!requestValidation)
            {
                context.Response.StatusCode = 400;
                _logger.LogWarning("Invalid request message to chat GPT", chatRequest);
                return;
            }
        }
        await _next(context);
    }

    private async Task<string> GetRequestMessageContent(HttpRequest request)
    {
        if (request.Method == HttpMethods.Post && request.ContentLength > 0)
        {
            request.EnableBuffering();

            var requestContent = await request.Body.ReadAsStringAsync(true);

            request.Body.Position = 0;

            return requestContent;
        }

        return "";
    }
}
