using Serilog;
using Serilog.Core;
using Serilog.Exceptions;

namespace ChatGptInjection.WebApi.Infrastructure.Extensions;

public static class LoggerExtensions
{
    /// <summary>
    ///     Configure the Serilog Logger with enrichers such as correlation id, exception details and sensitive data masking
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static Logger ConfigureLoggerWithEnrichers(this IConfiguration configuration)
    {
        var config = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails();

#if DEBUG
        config = config.WriteTo.Console();
#endif
        return config.CreateLogger();
    }
}
