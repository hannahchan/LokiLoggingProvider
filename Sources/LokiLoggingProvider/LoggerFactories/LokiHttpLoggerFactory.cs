namespace LokiLoggingProvider.LoggerFactories
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
    using Microsoft.Extensions.Logging;

    internal sealed class LokiHttpLoggerFactory : ILokiLoggerFactory
    {
        private readonly ConcurrentDictionary<string, LokiLogger> loggers = new ConcurrentDictionary<string, LokiLogger>();

        private readonly LokiLogMessageEntryProcessor processor;

        private readonly StaticLabelOptions staticLabelOptions;

        private readonly DynamicLabelOptions dynamicLabelOptions;

        private readonly FormatterOptions formatterOptions;

        private bool disposed;

        public LokiHttpLoggerFactory(
            HttpOptions httpOptions,
            StaticLabelOptions staticLabelOptions,
            DynamicLabelOptions dynamicLabelOptions,
            FormatterOptions formatterOptions)
        {
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(httpOptions.Address),
            };

            LokiHttpPushClient pushClient = new LokiHttpPushClient(httpClient);
            this.processor = new LokiLogMessageEntryProcessor(pushClient);

            this.staticLabelOptions = staticLabelOptions;
            this.dynamicLabelOptions = dynamicLabelOptions;
            this.formatterOptions = formatterOptions;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(LokiHttpLoggerFactory));
            }

            return this.loggers.GetOrAdd(categoryName, name => new LokiLogger(
                name,
                this.processor,
                this.staticLabelOptions,
                this.dynamicLabelOptions,
                this.formatterOptions));
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.processor.Dispose();
            this.disposed = true;
        }
    }
}
