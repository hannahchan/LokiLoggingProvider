namespace LokiLoggingProvider.LoggerFactories
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Http;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
    using Microsoft.Extensions.Logging;

    internal sealed class HttpLoggerFactory : ILokiLoggerFactory
    {
        private readonly ConcurrentDictionary<string, LokiLogger> loggers = new ConcurrentDictionary<string, LokiLogger>();

        private readonly ILogEntryFormatter formatter;

        private readonly LokiLogEntryProcessor processor;

        private readonly StaticLabelOptions staticLabelOptions;

        private readonly DynamicLabelOptions dynamicLabelOptions;

        private IExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

        private bool disposed;

        public HttpLoggerFactory(
            HttpOptions httpOptions,
            StaticLabelOptions staticLabelOptions,
            DynamicLabelOptions dynamicLabelOptions,
            Formatter formatter)
        {
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(httpOptions.Address),
            };

            HttpPushClient pushClient = new HttpPushClient(httpClient);
            this.processor = new LokiLogEntryProcessor(pushClient);

            this.staticLabelOptions = staticLabelOptions;
            this.dynamicLabelOptions = dynamicLabelOptions;

            this.formatter = formatter.CreateFormatter();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(HttpLoggerFactory));
            }

            return this.loggers.GetOrAdd(categoryName, name => new LokiLogger(
                name,
                this.formatter,
                this.processor,
                this.staticLabelOptions,
                this.dynamicLabelOptions)
            {
                ScopeProvider = this.scopeProvider,
            });
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

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;

            foreach (KeyValuePair<string, LokiLogger> logger in this.loggers)
            {
                logger.Value.ScopeProvider = this.scopeProvider;
            }
        }
    }
}
