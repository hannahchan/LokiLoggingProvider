namespace LokiLoggingProvider.Logger
{
    using System;
    using Microsoft.Extensions.Logging;

    internal class NullExternalScopeProvider : IExternalScopeProvider
    {
        private NullExternalScopeProvider()
        {
        }

        public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

        void IExternalScopeProvider.ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
            // Do nothing
        }

        IDisposable IExternalScopeProvider.Push(object state)
        {
            return NullScope.Instance;
        }
    }
}
