using Serilog;

namespace AppSec.AIPromtInjection.WebApi.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IHostBuilder ConfigureHostDefaultOptions(this WebApplicationBuilder builder)
    {
        return builder.Host
            .UseSerilog()
            .UseDefaultServiceProvider(options =>
                options.ValidateOnBuild = options.ValidateScopes = builder.Environment.IsDevelopment())
            .UseConsoleLifetime();
    }
}
