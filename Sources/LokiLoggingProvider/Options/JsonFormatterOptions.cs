namespace LokiLoggingProvider.Options
{
    public class JsonFormatterOptions
    {
        /// <summary>Includes the 'TraceId' key with the value set to the Activity Trace Identifier if present.</summary>
        public bool IncludeActivityTraceId { get; set; } = true;
    }
}
