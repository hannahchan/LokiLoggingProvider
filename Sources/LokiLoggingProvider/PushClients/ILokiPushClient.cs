namespace LokiLoggingProvider.PushClients;

using LokiLoggingProvider.Logger;

internal interface ILokiPushClient
{
    void Push(LokiLogEntry entry);
}
