using System;
using Expense.Application.Contracts;

namespace Expense.Infrastructure.Service
{
    //sealed → prevents other classes from inheriting from this class. This is standard for singletons.
    public sealed class LoggerService : ILoggerService
    {
        private static readonly Lazy<LoggerService> _instance = new Lazy<LoggerService>(() =>
            new LoggerService()
        );

        private LoggerService() { }

        public static LoggerService Instance => _instance.Value;

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }

        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }

        public void LogError(string message, Exception? ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            if (ex != null)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            Console.ResetColor();
        }
    }
}
