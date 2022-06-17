namespace LokiLoggingProvider.Logger;

using System;
using LokiLoggingProvider.Formatters;
using LokiLoggingProvider.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

internal class LokiLogger : ILogger
{
    private readonly string categoryName;

    private readonly ILogEntryFormatter formatter;

    private readonly ILokiLogEntryProcessor processor;

    private readonly LabelValues staticLabels;

    private readonly DynamicLabelOptions dynamicLabelOptions;

    public LokiLogger(
        string categoryName,
        ILogEntryFormatter formatter,
        ILokiLogEntryProcessor processor,
        StaticLabelOptions staticLabelOptions,
        DynamicLabelOptions dynamicLabelOptions)
    {
        this.categoryName = categoryName;
        this.formatter = formatter;
        this.processor = processor;

        this.staticLabels = new LabelValues(staticLabelOptions);
        this.dynamicLabelOptions = dynamicLabelOptions;
    }

    internal IExternalScopeProvider ScopeProvider { get; set; } = NullExternalScopeProvider.Instance;

    public IDisposable BeginScope<TState>(TState state)
    {
        return this.ScopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            return;
        }

        LogEntry<TState> logEntry = new(logLevel, this.categoryName, eventId, state, exception, formatter);

        DateTime timestamp = DateTime.UtcNow;
        LabelValues labels = this.staticLabels.AddDynamicLabels(this.dynamicLabelOptions, logEntry);
        string message = this.formatter.Format(logEntry, this.ScopeProvider);

        this.processor.EnqueueMessage(new LokiLogEntry(timestamp, labels, message));
    }
}
