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
            var chatRequest = JsonConvert.DeserializeObject<ChatMessageRequest>(requestMessageContent);
            var requestValidation = chatRequest is not null ? validator.ChatRequestValidate(chatRequest.Message) : false;
            if (!requestValidation)
            {
                context.Response.StatusCode = 400;
                _logger.LogWarning("Invalid request message to chat GPT: {@chatRequest}", chatRequest);
                return;
            }

            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);

                    var responseMessageContent = GetResponseMessageContent(memStream);
                    var chatResponse = JsonConvert.DeserializeObject<ChatMessageResponse>(responseMessageContent);
                    var responseValidation = chatResponse is not null ? validator.ChatResponseValidate(chatResponse.Message) : false;
                    if (!responseValidation)
                    {
                        context.Response.StatusCode = 400;
                        _logger.LogWarning("Invalid request message to chat GPT: {@chatRequest}", chatResponse);
                        return;
                    }

                    await memStream.CopyToAsync(originalBody);
                }

            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
        else
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

    private string GetResponseMessageContent(MemoryStream stream)
    {
        stream.Position = 0;
        var responseBody = new StreamReader(stream).ReadToEnd();
        stream.Position = 0;
        return responseBody;
    }
}
