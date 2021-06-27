namespace LokiLoggingProvider.Logger
{
    using System;
    using System.Collections.Generic;
    using LokiLoggingProvider.Labels;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;

    internal class LokiLogger : ILogger
    {
        private readonly string categoryName;

        private readonly LokiLogMessageEntryProcessor processor;

        private readonly IReadOnlyDictionary<string, string> staticLabels;

        private readonly DynamicLabelOptions dynamicLabelOptions;

        private readonly FormatterOptions formatterOptions;

        internal LokiLogger(
            string categoryName,
            LokiLogMessageEntryProcessor processor,
            StaticLabelOptions staticLabelOptions,
            DynamicLabelOptions dynamicLabelOptions,
            FormatterOptions formatterOptions)
        {
            this.categoryName = categoryName;
            this.processor = processor;

            this.staticLabels = staticLabelOptions.ToReadOnlyDictionary();
            this.dynamicLabelOptions = dynamicLabelOptions;
            this.formatterOptions = formatterOptions;
        }

        public IDisposable? BeginScope<TState>(TState state)
        {
            return default;
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

            IReadOnlyDictionary<string, string> labels = this.staticLabels.AddDynamicLables(
                this.dynamicLabelOptions,
                this.categoryName,
                logLevel,
                eventId,
                exception);

            this.processor.EnqueueMessage(new LokiLogMessageEntry(
                timestamp: DateTime.UtcNow,
                labels: labels,
                message: formatter(state, exception)));
        }
    }
}
