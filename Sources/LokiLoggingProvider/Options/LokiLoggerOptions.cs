namespace LokiLoggingProvider.Options;

public class LokiLoggerOptions
{
    /// <summary>The push client to use. Valid values are 'None', 'Grpc' or 'Http'. Defaults to 'None'.</summary>
    public PushClient Client { get; set; } = PushClient.None;

    /// <summary>Configures the gRPC push client.</summary>
    public GrpcOptions Grpc { get; set; } = new GrpcOptions();

    /// <summary>Configures the HTTP push client.</summary>
    public HttpOptions Http { get; set; } = new HttpOptions();

    /// <summary>Configures and adds static labels.</summary>
    public StaticLabelOptions StaticLabels { get; set; } = new StaticLabelOptions();

    /// <summary>Configures and adds dynamic labels. Overrides static labels if they clash. Use with caution.</summary>
    public DynamicLabelOptions DynamicLabels { get; set; } = new DynamicLabelOptions();

    /// <summary>The formatter to use. Valid values are 'Simple', 'Json' or 'Logfmt'. Defaults to 'Simple'.</summary>
    public Formatter Formatter { get; set; } = Formatter.Simple;

    /// <summary>Configures the 'Simple' Formatter.</summary>
    public SimpleFormatterOptions SimpleFormatter { get; set; } = new SimpleFormatterOptions();

    /// <summary>Configures the 'Json' Formatter.</summary>
    public JsonFormatterOptions JsonFormatter { get; set; } = new JsonFormatterOptions();

    /// <summary>Configures the 'Logfmt' Formatter.</summary>
    public LogfmtFormatterOptions LogfmtFormatter { get; set; } = new LogfmtFormatterOptions();
}
