namespace LokiLoggingProvider.Logger
{
    using System;

    internal sealed class NullScope : IDisposable
    {
        private NullScope()
        {
        }

        public static NullScope Instance { get; } = new NullScope();

        public void Dispose()
        {
            // Do nothing
        }
    }
}
