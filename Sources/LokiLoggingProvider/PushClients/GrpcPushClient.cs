namespace LokiLoggingProvider.PushClients;

using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Logproto;
using LokiLoggingProvider.Logger;

internal class GrpcPushClient : ILokiPushClient
{
    private readonly Pusher.PusherClient client;

    public GrpcPushClient(GrpcChannel channel)
    {
        this.client = new Pusher.PusherClient(channel);
    }

    public void Push(LokiLogEntry entry)
    {
        StreamAdapter stream = new()
        {
            Labels = $"{{{entry.Labels}}}",
        };

        stream.Entries.Add(new EntryAdapter
        {
            Timestamp = Timestamp.FromDateTime(entry.Timestamp),
            Line = entry.Message,
        });

        PushRequest request = new();
        request.Streams.Add(stream);

        this.client.Push(request);
    }
}
