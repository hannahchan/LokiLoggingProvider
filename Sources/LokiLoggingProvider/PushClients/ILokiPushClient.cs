namespace LokiLoggingProvider.PushClients
{
    using System;
    using LokiLoggingProvider.Logger;

    internal interface ILokiPushClient : IDisposable
    {
        void Push(LokiLogMessageEntry entry);
    }
}
