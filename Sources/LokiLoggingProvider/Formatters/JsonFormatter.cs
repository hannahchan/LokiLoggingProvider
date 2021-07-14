namespace LokiLoggingProvider.Formatters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class JsonFormatter : ILogEntryFormatter
    {
        private readonly JsonFormatterOptions formatterOptions;

        private readonly JsonSerializerOptions serializerOptions;

        public JsonFormatter(JsonFormatterOptions formatterOptions)
        {
            this.formatterOptions = formatterOptions;

            this.serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = this.formatterOptions.WriteIndented,
            };
        }

        public string Format<TState>(LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider = null)
        {
            LogValues logValues = new LogValues();
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

            if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> state && state.Any())
            {
                try
                {
                    logValues.SetState(new Dictionary<string, object?>(state));
                }
                catch
                {
                    logValues.SetState(state);
                }
            }

            if (this.formatterOptions.IncludeScopes && scopeProvider != null)
            {
                List<object> scopes = new List<object>();

                scopeProvider.ForEachScope(
                    (scope, state) =>
                    {
                        if (scope is IEnumerable<KeyValuePair<string, object?>> keyValuePairs)
                        {
                            try
                            {
                                state.Add(new Dictionary<string, object?>(keyValuePairs));
                                return;
                            }
                            catch
                            {
                                state.Add(keyValuePairs);
                                return;
                            }
                        }

                        state.Add(scope);
                    },
                    scopes);

                if (scopes.Any())
                {
                    logValues.SetScopes(scopes);
                }
            }

            if (logEntry.Exception != null)
            {
                logValues.SetException(logEntry.Exception.GetType().ToString());
                logValues.SetExceptionDetails(logEntry.Exception.ToString());
            }

            if (this.formatterOptions.IncludeActivityTracking)
            {
                logValues.AddActivityTracking();
            }

            return JsonSerializer.Serialize(logValues, this.serializerOptions);
        }
    }
}
