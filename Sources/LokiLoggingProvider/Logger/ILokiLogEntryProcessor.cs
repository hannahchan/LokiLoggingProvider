namespace LokiLoggingProvider.Logger
{
    using System;

    internal interface ILokiLogEntryProcessor : IDisposable
    {
        void EnqueueMessage(LokiLogEntry message);
    }
}
