namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class LogfmtFormatter : ILogEntryFormatter
    {
        private readonly LogfmtFormatterOptions formatterOptions;

        public LogfmtFormatter(LogfmtFormatterOptions formatterOptions)
        {
            this.formatterOptions = formatterOptions;
        }

        public string Format<TState>(LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider = null)
        {
            LogValues logValues = new();
            logValues.SetLogLevel(logEntry.LogLevel.ToString());

            if (this.formatterOptions.IncludeCategory)
            {
                logValues.SetCategory(logEntry.Category);
            }

            if (this.formatterOptions.IncludeEventId)
            {
                logValues.SetEventId(logEntry.EventId.Id);
            }

            logValues.SetMessage(logEntry.Formatter(logEntry.State, logEntry.Exception));

            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> state)
            {
                foreach (KeyValuePair<string, object?> keyValuePair in state)
                {
                    logValues.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }

            if (this.formatterOptions.IncludeScopes && scopeProvider != null)
            {
                scopeProvider.ForEachScope(
                    (scope, state) =>
                    {
                        if (scope is IEnumerable<KeyValuePair<string, object?>> keyValuePairs)
                        {
                            foreach (KeyValuePair<string, object?> keyValuePair in keyValuePairs)
                            {
                                state.TryAdd(keyValuePair.Key, keyValuePair.Value);
                            }
                        }
                    },
                    logValues);
            }

            if (logEntry.Exception != null)
            {
                logValues.SetException(logEntry.Exception.GetType());
            }

            if (this.formatterOptions.IncludeActivityTracking)
            {
                logValues.AddActivityTracking();
            }

            string message = string.Join(" ", logValues.Select(keyValuePair => $"{ToLogfmtKey(keyValuePair.Key)}={ToLogfmtValue(keyValuePair.Value)}"));

            if (logEntry.Exception != null && this.formatterOptions.PrintExceptions)
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
