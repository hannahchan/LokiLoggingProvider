namespace LokiLoggingProvider.Logger
{
    using System;
    using System.Collections.Generic;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class LokiLogger : ILogger
    {
        private readonly string categoryName;

        private readonly ILogEntryFormatter formatter;

        private readonly LokiLogEntryProcessor processor;

        private readonly IReadOnlyDictionary<string, string> staticLabels;

        private readonly DynamicLabelOptions dynamicLabelOptions;

        public LokiLogger(
            string categoryName,
            ILogEntryFormatter formatter,
            LokiLogEntryProcessor processor,
            StaticLabelOptions staticLabelOptions,
            DynamicLabelOptions dynamicLabelOptions)
        {
            this.categoryName = categoryName;
            this.formatter = formatter;
            this.processor = processor;

            this.staticLabels = staticLabelOptions.ToReadOnlyDictionary();
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

            DateTime timestamp = DateTime.UtcNow;

            IReadOnlyDictionary<string, string> labels = this.staticLabels.AddDynamicLables(
                this.dynamicLabelOptions,
                this.categoryName,
                logLevel,
                eventId,
                exception);

            LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, this.categoryName, eventId, state, exception, formatter);

            this.processor.EnqueueMessage(new LokiLogEntry(timestamp, labels, this.formatter.Format(logEntry)));
        }
    }
}
