namespace LokiLoggingProvider.LoggerFactories
{
    using System;
    using System.Collections.Concurrent;
    using Grpc.Net.Client;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
    using Microsoft.Extensions.Logging;

    internal sealed class LokiGrpcLoggerFactory : ILokiLoggerFactory
    {
        private readonly ConcurrentDictionary<string, LokiLogger> loggers = new ConcurrentDictionary<string, LokiLogger>();

        private readonly LokiLogMessageEntryProcessor processor;

        private readonly StaticLabelOptions staticLabelOptions;

        private readonly DynamicLabelOptions dynamicLabelOptions;

        private readonly FormatterOptions formatterOptions;

        private bool disposed;

        public LokiGrpcLoggerFactory(
            GrpcOptions grpcOptions,
            StaticLabelOptions staticLabelOptions,
            DynamicLabelOptions dynamicLabelOptions,
            FormatterOptions formatterOptions)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(grpcOptions.Address);
            LokiGrpcPushClient grpcClient = new LokiGrpcPushClient(channel);
            this.processor = new LokiLogMessageEntryProcessor(grpcClient);

            this.staticLabelOptions = staticLabelOptions;
            this.dynamicLabelOptions = dynamicLabelOptions;
            this.formatterOptions = formatterOptions;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(LokiGrpcLoggerFactory));
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
