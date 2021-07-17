namespace LokiLoggingProvider.Logger
{
    using System;
    using Microsoft.Extensions.Logging;

    internal class NullExternalScopeProvider : IExternalScopeProvider
    {
        private NullExternalScopeProvider()
        {
        }

        public static NullExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
            // Do nothing
        }

        public IDisposable Push(object state)
        {
            return NullScope.Instance;
        }
    }
}
