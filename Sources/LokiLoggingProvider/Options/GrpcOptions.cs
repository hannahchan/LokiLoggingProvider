namespace LokiLoggingProvider.Options;

public class GrpcOptions
{
    /// <summary>The destination to send logs to. Defaults to "http://localhost:9095".</summary>
    public string Address { get; set; } = "http://localhost:9095";
}
