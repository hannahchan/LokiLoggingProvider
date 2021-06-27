namespace LokiLoggingProvider.LoggerFactories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal sealed class LokiNullLoggerFactory : ILokiLoggerFactory
    {
        public ILogger CreateLogger(string categoryName)
        {
            return NullLogger.Instance;
        }

        public void Dispose()
        {
            // Do nothing
        }
    }
}
