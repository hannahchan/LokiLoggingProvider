namespace LokiLoggingProvider.Options
{
    public class LokiLoggerOptions
    {
        /// <summary>The push client to use. Valid values are 'None', 'Grpc' or 'Http'. Defaults to 'None'.</summary>
        public PushClient Client { get; set; } = PushClient.None;

        /// <summary>Configures the gRPC push client.</summary>
        public GrpcOptions GrpcOptions { get; set; } = new GrpcOptions();

        /// <summary>Configures the HTTP push client.</summary>
        public HttpOptions HttpOptions { get; set; } = new HttpOptions();

        /// <summary>Configures and adds static labels.</summary>
        public StaticLabelOptions StaticLabelOptions { get; set; } = new StaticLabelOptions();

        /// <summary>Configures and adds dynamic labels. Use with caution.</summary>
        public DynamicLabelOptions DynamicLabelOptions { get; set; } = new DynamicLabelOptions();

        public FormatterOptions FormatterOptions { get; set; } = new FormatterOptions();
    }
}
