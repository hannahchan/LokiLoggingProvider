namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Diagnostics;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class SimpleFormatter : ILogEntryFormatter
    {
        private readonly SimpleFormatterOptions options;

        public SimpleFormatter(SimpleFormatterOptions options)
        {
            this.options = options;
        }

        public string Format<TState>(LogEntry<TState> logEntry)
        {
            string message = $"[{GetLogLevelString(logEntry.LogLevel)}] ";

            if (this.options.IncludeActivityTracking && Activity.Current is Activity activity)
            {
                message += $"{activity.GetTraceId()} - ";
            }

            message += logEntry.Formatter(logEntry.State, logEntry.Exception);

            if (logEntry.Exception != null)
            {
                message += Environment.NewLine + logEntry.Exception.ToString();
            }

            return message;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "TRCE",
                LogLevel.Debug => "DBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "EROR",
                LogLevel.Critical => "CRIT",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
            };
        }
    }
}
