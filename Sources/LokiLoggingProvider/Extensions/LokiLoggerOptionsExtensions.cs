namespace LokiLoggingProvider.Extensions
{
    using System;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.LoggerFactories;
    using LokiLoggingProvider.Options;

    internal static class LokiLoggerOptionsExtensions
    {
        public static ILogEntryFormatter CreateFormatter(this LokiLoggerOptions options)
        {
            return options.Formatter switch
            {
                Formatter.Json => throw new NotImplementedException(),
                Formatter.Logfmt => new LogfmtFormatter(options.LogfmtFormatter),
                _ => new SimpleFormatter(options.SimpleFormatter),
            };
        }

        public static ILokiLoggerFactory CreateLoggerFactory(this LokiLoggerOptions options)
        {
            ILogEntryFormatter formatter = options.CreateFormatter();

            return options.Client switch
            {
                PushClient.Grpc => new GrpcLoggerFactory(
                    options.Grpc,
                    options.StaticLabels,
                    options.DynamicLabels,
                    formatter),

                PushClient.Http => new HttpLoggerFactory(
                    options.Http,
                    options.StaticLabels,
                    options.DynamicLabels,
                    formatter),

                _ => new NullLoggerFactory(),
            };
        }
    }
}
