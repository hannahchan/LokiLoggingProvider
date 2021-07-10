namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
            Dictionary<string, object?> keyValuePairs = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["LogLevel"] = logEntry.LogLevel,
            };

            if (this.options.IncludeCategory)
            {
                keyValuePairs["Category"] = logEntry.Category;
            }

            if (this.options.IncludeEventId)
            {
                keyValuePairs["EventId"] = logEntry.EventId;
            }

            keyValuePairs["Message"] = logEntry.Formatter(logEntry.State, logEntry.Exception);

            if (logEntry.Exception != null)
            {
                keyValuePairs["Exception"] = logEntry.Exception.GetType();
            }

            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> state)
            {
                foreach (KeyValuePair<string, object?> keyValuePair in state)
                {
                    keyValuePairs.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }

            if (this.options.IncludeActivityTracking && Activity.Current is Activity activity)
            {
                keyValuePairs.TryAdd("SpanId", activity.GetSpanId());
                keyValuePairs.TryAdd("TraceId", activity.GetTraceId());
                keyValuePairs.TryAdd("ParentId", activity.GetParentId());
            }

            string message = string.Join(" ", keyValuePairs.Select(keyValuePair => $"{ToLogfmtKey(keyValuePair.Key)}={ToLogfmtValue(keyValuePair.Value)}"));

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
