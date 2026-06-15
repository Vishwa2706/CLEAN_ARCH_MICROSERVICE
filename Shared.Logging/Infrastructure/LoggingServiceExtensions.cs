using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Logging.Contracts;

namespace Shared.Logging.Infrastructure;

public static class LoggingServiceExtensions
{
    public static void AddSharedLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName
    )
    {
        services.AddSingleton<ILogger>(
            SharedLoggerConfiguration.CreateLoggerInstance(configuration, serviceName)
        );

        services.AddSingleton(typeof(ILoggerManager<>), typeof(LoggerManager<>));
    }
}
