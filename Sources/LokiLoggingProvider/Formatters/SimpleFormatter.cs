namespace LokiLoggingProvider.Formatters
{
    using System;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class SimpleFormatter : ILogEntryFormatter
    {
        public string Format<TState>(LogEntry<TState> logEntry)
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);

            if (logEntry.Exception != null)
            {
                message = message + Environment.NewLine + logEntry.Exception.ToString();
            }

            return message;
        }
    }
}
