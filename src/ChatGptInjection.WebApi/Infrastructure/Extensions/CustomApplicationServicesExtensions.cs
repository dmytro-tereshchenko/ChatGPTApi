using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Services;
using ChatGptInjection.Services.Azure;
using ChatGptInjection.Services.OpenAI;
using ChatGptInjection.Services.Validators;

namespace ChatGptInjection.WebApi.Infrastructure.Extensions;

public static class CustomApplicationServicesExtensions
{
    public static IServiceCollection AddCustomApplicationServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddHttpClient("OpenAI", client =>
        {
            client.BaseAddress = new Uri(appSettings.OpenAiHost);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        });

        return services
            .AddTransient<IChatContentValidator, ChatContentValidator>()
            .AddTransient<IChatGptService, ChatGptService>()
            .AddTransient<IBlobStorageService, BlobStorageService>();
    }
}
