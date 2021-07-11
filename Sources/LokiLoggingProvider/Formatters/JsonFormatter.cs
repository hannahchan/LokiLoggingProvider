namespace LokiLoggingProvider.Formatters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class JsonFormatter : ILogEntryFormatter
    {
        private readonly JsonFormatterOptions options;

        public JsonFormatter(JsonFormatterOptions options)
        {
            this.options = options;
        }

        public string Format<TState>(LogEntry<TState> logEntry)
        {
            LogValues logValues = new LogValues
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

            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> state && state.Any())
            {
                Dictionary<string, object?> stateValues = new Dictionary<string, object?>();

                foreach (KeyValuePair<string, object?> keyValuePair in state)
                {
                    stateValues.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }

                logValues.State = stateValues;
            }

            if (logEntry.Exception != null)
            {
                logValues.Exception = logEntry.Exception.GetType().ToString();
                logValues.ExceptionDetails = logEntry.Exception.ToString();
            }

            if (this.options.IncludeActivityTracking)
            {
                logValues.AddActivityTracking();
            }

            return JsonSerializer.Serialize(logValues);
        }
    }
}
