using AppSec.AIPromtInjection.Abstractions;
using AppSec.AIPromtInjection.Abstractions.Services;
using AppSec.AIPromtInjection.Services.OpenAI;
using AppSec.AIPromtInjection.Services.Validators;

namespace AppSec.AIPromtInjection.WebApi.Infrastructure.Extensions;

public static class CustomApplicationServicesExtensions
{
    public static IServiceCollection AddCustomApplicationServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddHttpClient("OpenAI", client =>
        {
            client.BaseAddress = new Uri(appSettings.OpenAiHost);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services
            .AddTransient<IChatContentValidator, ChatContentValidator>()
            .AddTransient<IChatGptService, ChatGptService>();
    }
}
