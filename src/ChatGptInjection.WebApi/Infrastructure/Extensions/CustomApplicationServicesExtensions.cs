using ChatGptInjection.Abstractions.Services;
using ChatGptInjection.Services.Azure;
using ChatGptInjection.Services.Handlers;
using ChatGptInjection.Services.OpenAI;
using ChatGptInjection.Services.Validators;

namespace ChatGptInjection.WebApi.Infrastructure.Extensions;

public static class CustomApplicationServicesExtensions
{
    public static IServiceCollection AddCustomApplicationServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IChatContentValidator, ChatContentValidator>()
            .AddTransient<IChatGptService, ChatGptService>()
            .AddTransient<IBlobStorageService, BlobStorageService>()
            .AddTransient<ITableStorageService, TableStorageService>()
            .AddTransient<IHistoryHandler, HistoryHandler>()
            .AddTransient<IChatGptCommandHandler, ChatGptCommandHandler>();
    }
}
