namespace LokiLoggingProvider.LoggerFactories
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
    using Microsoft.Extensions.Logging;

    internal sealed class HttpLoggerFactory : ILokiLoggerFactory
    {
        private readonly ConcurrentDictionary<string, LokiLogger> loggers = new();

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
            ILogEntryFormatter formatter)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(httpOptions.Address),
            };

            if (!string.IsNullOrEmpty(httpOptions.User) && !string.IsNullOrEmpty(httpOptions.Password))
            {
                byte[] credentials = Encoding.ASCII.GetBytes($"{httpOptions.User}:{httpOptions.Password}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            }

            HttpPushClient pushClient = new(httpClient);
            this.processor = new LokiLogEntryProcessor(pushClient);

            this.staticLabelOptions = staticLabelOptions;
            this.dynamicLabelOptions = dynamicLabelOptions;

            this.formatter = formatter;
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
