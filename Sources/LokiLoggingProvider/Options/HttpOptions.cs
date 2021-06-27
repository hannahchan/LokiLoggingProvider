namespace LokiLoggingProvider.Options
{
    public class HttpOptions
    {
        /// <summary>The destination to send logs to. Defaults to "http://localhost:3100".</summary>
        public string Address { get; set; } = "http://localhost:3100";
    }
}
