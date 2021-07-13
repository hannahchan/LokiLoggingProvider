namespace LokiLoggingProvider.Options
{
    public class JsonFormatterOptions
    {
        /// <summary>Includes 'SpanId', 'TraceId' and 'ParentId' if present in the current Activity.</summary>
        public bool IncludeActivityTracking { get; set; } = true;

        /// <summary>Includes the 'Category' key with the value set to the Category Name of the logger.</summary>
        public bool IncludeCategory { get; set; }

        /// <summary>Includes the 'EventId' key with the value set to the Event ID of the log entry.</summary>
        public bool IncludeEventId { get; set; }

        /// <summary>Includes log scopes in the formatted output.</summary>
        public bool IncludeScopes { get; set; }

        /// <summary>Writes indented JSON.</summary>
        public bool WriteIndented { get; set; }
    }
}
