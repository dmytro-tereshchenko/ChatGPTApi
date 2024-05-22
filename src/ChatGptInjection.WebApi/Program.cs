using ChatGptInjection.Abstractions;
using ChatGptInjection.WebApi.Infrastructure.Extensions;
using Serilog;

var configuration = BuildConfiguration();
Log.Logger = configuration.ConfigureLoggerWithEnrichers();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.ConfigureHostDefaultOptions();
    var services = builder.Services;

    services.Configure<AppSettings>(configuration);
    services.AddCustomApplicationServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler();
    }

    app.UseSerilogRequestLogging(options => { options.IncludeQueryInRequestPath = true; });

    app.UseChatContentValidationMiddleware();

    app.RegisterRoutes();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host failed to start");
}
finally
{
    Log.Information("Host shut down completed");
    Log.CloseAndFlush();
}

static IConfigurationRoot BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables()
        .Build();
}
