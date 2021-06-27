namespace Microsoft.Extensions.Logging
{
    using LokiLoggingProvider;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;

    public static class LokiLoggerExtensions
    {
        public static ILoggingBuilder AddLoki(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services
                .AddOptions()
                .TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LokiLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions<LokiLoggerOptions, LokiLoggerProvider>(builder.Services);

            return builder;
        }
    }
}
