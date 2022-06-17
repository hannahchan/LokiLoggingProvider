namespace LokiLoggingProvider.Options;

public enum PushClient
{
    /// <summary>Used to disable logging.</summary>
    None = 0,

    /// <summary>Represents a GRPC push client.</summary>
    Grpc = 1,

    /// <summary>Represents a HTTP push client.</summary>
    Http = 2,
}
