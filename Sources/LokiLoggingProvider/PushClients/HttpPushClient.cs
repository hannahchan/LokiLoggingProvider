namespace LokiLoggingProvider.PushClients;

using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using LokiLoggingProvider.Logger;

internal sealed class HttpPushClient : ILokiPushClient
{
    private const string PushEndpointV1 = "/loki/api/v1/push";

    private readonly HttpClient client;

    public HttpPushClient(HttpClient client)
    {
        this.client = client;
    }

    public void Push(LokiLogEntry entry)
    {
        Timestamp rfc3339Timestamp = Timestamp.FromDateTime(entry.Timestamp);

        var requestBody = new
        {
            streams = new[]
            {
                new
                {
                    stream = entry.Labels,
                    values = new[]
                    {
                        new[]
                        {
                            $"{rfc3339Timestamp.Seconds}{rfc3339Timestamp.Nanos:000000000}",
                            entry.Message,
                        },
                    },
                },
            },
        };

        StringContent content = new(JsonSerializer.Serialize(requestBody), null, MediaTypeNames.Application.Json);
        content.Headers.ContentType!.CharSet = null; // Loki does not accept 'charset' in the Content-Type header

        HttpRequestMessage request = new(HttpMethod.Post, PushEndpointV1)
        {
            Content = content,
        };

        this.client.Send(request).Dispose();
    }
}
