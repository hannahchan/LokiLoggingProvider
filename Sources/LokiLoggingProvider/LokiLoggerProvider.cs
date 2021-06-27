namespace LokiLoggingProvider
{
    using System;
    using LokiLoggingProvider.LoggerFactories;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [ProviderAlias("Loki")]
    public sealed class LokiLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable onChangeToken;

        private bool disposed;

        private ILokiLoggerFactory loggerFactory;

        public LokiLoggerProvider(IOptionsMonitor<LokiLoggerOptions> options)
        {
            this.loggerFactory = CreateLoggerFactory(options.CurrentValue);

            this.onChangeToken = options.OnChange(updatedOptions =>
            {
                this.loggerFactory.Dispose();
                this.loggerFactory = CreateLoggerFactory(updatedOptions);
            });
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(LokiLoggerProvider));
            }

            return this.loggerFactory.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.onChangeToken.Dispose();
            this.loggerFactory.Dispose();

            this.disposed = true;
        }

        private static ILokiLoggerFactory CreateLoggerFactory(LokiLoggerOptions options)
        {
            switch (options.Client)
            {
                case PushClient.Grpc:
                    return new LokiGrpcLoggerFactory(
                        options.GrpcOptions,
                        options.StaticLabelOptions,
                        options.DynamicLabelOptions,
                        options.FormatterOptions);

                case PushClient.Http:
                    return new LokiHttpLoggerFactory(
                        options.HttpOptions,
                        options.StaticLabelOptions,
                        options.DynamicLabelOptions,
                        options.FormatterOptions);

                case PushClient.None:
                default:
                    return new LokiNullLoggerFactory();
            }
        }
    }
}
