namespace LokiLoggingProvider.PushClients
{
    using System;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Net.Client;
    using Logproto;
    using LokiLoggingProvider.Logger;

    internal sealed class GrpcPushClient : ILokiPushClient
    {
        private readonly GrpcChannel channel;

        private readonly Pusher.PusherClient client;

        private bool disposed;

        public GrpcPushClient(GrpcChannel channel)
        {
            this.channel = channel;
            this.client = new Pusher.PusherClient(this.channel);
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.channel.Dispose();
            this.disposed = true;
        }

        public void Push(LokiLogEntry entry)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(GrpcPushClient));
            }

            StreamAdapter stream = new StreamAdapter
            {
                Labels = entry.Labels.ToLabelString(),
            };

            stream.Entries.Add(new EntryAdapter
            {
                Timestamp = Timestamp.FromDateTime(entry.Timestamp),
                Line = entry.Message,
            });

            PushRequest request = new PushRequest();
            request.Streams.Add(stream);

            this.client.Push(request);
        }
    }
}
