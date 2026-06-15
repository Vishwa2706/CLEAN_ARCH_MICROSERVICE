using Serilog;
using Shared.Logging.Contracts;

namespace Shared.Logging.Infrastructure;

public class LoggerManager<T> : ILoggerManager<T>
{
    private readonly ILogger _logger;

    public LoggerManager(ILogger logger)
    {
        _logger = logger.ForContext<T>();
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
            _logger.Error(exception, message, args);
        else
            _logger.Error(message, args);
    }

    public void LogCritical(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
            _logger.Fatal(exception, message, args);
        else
            _logger.Fatal(message, args);
    }
}
