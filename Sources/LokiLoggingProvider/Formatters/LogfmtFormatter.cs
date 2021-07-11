namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class LogfmtFormatter : ILogEntryFormatter
    {
        private readonly LogfmtFormatterOptions options;

        public LogfmtFormatter(LogfmtFormatterOptions options)
        {
            this.options = options;
        }

        public string Format<TState>(LogEntry<TState> logEntry)
        {
            LogValues logValues = new LogValues()
            {
                LogLevel = logEntry.LogLevel.ToString(),
            };

            if (this.options.IncludeCategory)
            {
                logValues.Category = logEntry.Category;
            }

            if (this.options.IncludeEventId)
            {
                logValues.EventId = logEntry.EventId.Id;
            }

            logValues.Message = logEntry.Formatter(logEntry.State, logEntry.Exception);

            if (logEntry.Exception != null)
            {
                logValues.Exception = logEntry.Exception.GetType();
            }

            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> state)
            {
                foreach (KeyValuePair<string, object?> keyValuePair in state)
                {
                    logValues.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }

            if (this.options.IncludeActivityTracking)
            {
                logValues.AddActivityTracking();
            }

            string message = string.Join(" ", logValues.Select(keyValuePair => $"{ToLogfmtKey(keyValuePair.Key)}={ToLogfmtValue(keyValuePair.Value)}"));

            if (logEntry.Exception != null && this.options.PrintExceptions)
            {
                message += Environment.NewLine + logEntry.Exception.ToString();
            }

            return message;
        }

        private static string ToLogfmtKey(string key)
        {
            return key.Replace(" ", string.Empty);
        }

        private static string ToLogfmtValue(object? value)
        {
            string? stringValue = value?.ToString();

            if (string.IsNullOrEmpty(stringValue))
            {
                return "\"\"";
            }

            if (stringValue.Contains(" "))
            {
                return $"\"{stringValue}\"";
            }

            return stringValue;
        }
    }
}
