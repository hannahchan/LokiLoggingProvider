namespace LokiLoggingProvider.Formatters
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class LogfmtFormatter : ILogEntryFormatter
    {
        public string Format<TState>(LogEntry<TState> logEntry)
        {
            LogLevel logLevel = logEntry.LogLevel;
            string category = logEntry.Category;
            EventId eventId = logEntry.EventId;
            TState state = logEntry.State;
            Exception? exception = logEntry.Exception;
            Func<TState, Exception?, string> formatter = logEntry.Formatter;

            string message = $"logLevel={logLevel} category={category} eventId={eventId} message=\"{formatter(state, exception)}\"";

            if (exception != null)
            {
                message += $" exception={exception.GetType()}" + Environment.NewLine + exception.ToString();
            }

            return message;
        }
    }
}
