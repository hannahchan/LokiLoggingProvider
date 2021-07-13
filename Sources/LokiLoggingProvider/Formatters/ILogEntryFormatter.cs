namespace LokiLoggingProvider.Formatters
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal interface ILogEntryFormatter
    {
        string Format<TState>(LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider = null);
    }
}
