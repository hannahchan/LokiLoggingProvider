namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class LogfmtFormatter : ILogEntryFormatter
    {
        public string Format<TState>(LogEntry<TState> logEntry)
        {
            List<KeyValuePair<string, object?>> keyValuePairs = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("logLevel", logEntry.LogLevel),
                new KeyValuePair<string, object?>("category", logEntry.Category),
                new KeyValuePair<string, object?>("eventId", logEntry.EventId),
                new KeyValuePair<string, object?>("message", logEntry.Formatter(logEntry.State, logEntry.Exception)),
            };

            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> state)
            {
                keyValuePairs.AddRange(state);
            }

            if (logEntry.Exception != null)
            {
                keyValuePairs.Add(new KeyValuePair<string, object?>("exception", logEntry.Exception.GetType()));
            }

            IEnumerable<string> logfmtKeyValuePairs = keyValuePairs.Select(keyValuePair => $"{keyValuePair.Key}={ToLogfmtValue(keyValuePair.Value)}");
            string message = string.Join(" ", logfmtKeyValuePairs);

            if (logEntry.Exception != null)
            {
                message += Environment.NewLine + logEntry.Exception.ToString();
            }

            return message;
        }

        private static string ToLogfmtValue(object? @object)
        {
            string? @string = @object?.ToString();

            if (string.IsNullOrEmpty(@string))
            {
                return "\"\"";
            }

            if (@string.Contains(" "))
            {
                return $"\"{@string}\"";
            }

            return @string;
        }
    }
}
