namespace Shared.Logging.Contracts;

public interface ILoggerManager<T>
{
    void LogInformation(string message, params object[] args);

    void LogWarning(string message, params object[] args);

    void LogError(string message, Exception? exception = null, params object[] args);

    void LogDebug(string message, params object[] args);

    void LogCritical(string message, Exception? exception = null, params object[] args);
}
