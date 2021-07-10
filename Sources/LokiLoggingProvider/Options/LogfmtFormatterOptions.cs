namespace LokiLoggingProvider.Options
{
    public class LogfmtFormatterOptions
    {
        /// <summary>Includes 'SpanId', 'TraceId' and 'ParentId' if present in the current Activity.</summary>
        public bool IncludeActivityTracking { get; set; } = true;

        /// <summary>Includes the 'Category' key with the value set to the Category Name of the logger.</summary>
        public bool IncludeCategory { get; set; }

        /// <summary>Includes the 'EventId' key with the value set to the Event ID of the log entry.</summary>
        public bool IncludeEventId { get; set; }

        public bool IncludeScopes { get; set; }

        /// <summary>Appends exception details to the end of the log entry.</summary>
        public bool PrintExceptions { get; set; } = true;
    }
}
