using Microsoft.Extensions.Configuration;
using Serilog;

namespace Shared.Logging.Infrastructure;

public static class SharedLoggerConfiguration
{
    public static ILogger CreateLoggerInstance(IConfiguration configuration, string serviceName)
    {
        var logPath = configuration["Serilog:LogPath"] ?? "Logs";

        var outputTemplate =
            configuration["Serilog:OutputTemplate"]
            ?? "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(
                path: Path.Combine(logPath, $"{serviceName}-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: outputTemplate
            )
            .CreateLogger();
    }
}
