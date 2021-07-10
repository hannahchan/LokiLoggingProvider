namespace LokiLoggingProvider.LoggerFactories
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Grpc.Net.Client;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
    using Microsoft.Extensions.Logging;

    internal sealed class GrpcLoggerFactory : ILokiLoggerFactory
    {
        private readonly ConcurrentDictionary<string, LokiLogger> loggers = new ConcurrentDictionary<string, LokiLogger>();

        private readonly ILogEntryFormatter formatter;

        private readonly LokiLogEntryProcessor processor;

        private readonly StaticLabelOptions staticLabelOptions;

        private readonly DynamicLabelOptions dynamicLabelOptions;

        private IExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

        private bool disposed;

        public GrpcLoggerFactory(
            GrpcOptions grpcOptions,
            StaticLabelOptions staticLabelOptions,
            DynamicLabelOptions dynamicLabelOptions,
            ILogEntryFormatter formatter)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(grpcOptions.Address);
            GrpcPushClient grpcClient = new GrpcPushClient(channel);
            this.processor = new LokiLogEntryProcessor(grpcClient);

            this.staticLabelOptions = staticLabelOptions;
            this.dynamicLabelOptions = dynamicLabelOptions;

            this.formatter = formatter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(GrpcLoggerFactory));
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
