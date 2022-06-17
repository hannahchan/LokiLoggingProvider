namespace LokiLoggingProvider.Options;

public class SimpleFormatterOptions
{
    /// <summary>Includes the Activity Trace Identifier if present in the log message.</summary>
    public bool IncludeActivityTracking { get; set; } = true;
}
