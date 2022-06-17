namespace LokiLoggingProvider.Options;

public class DynamicLabelOptions
{
    /// <summary>Includes the label 'category' with the value set to the Category Name of the logger.</summary>
    public bool IncludeCategory { get; set; }

    /// <summary>Includes the label 'logLevel' with the value set to the Log Level of the log entry.</summary>
    public bool IncludeLogLevel { get; set; }

    /// <summary>Includes the label 'eventId' with the value set to the Event ID of the log entry.</summary>
    public bool IncludeEventId { get; set; }

    /// <summary>Includes the label 'exception' with the value set to name of the exception if present in the log entry.</summary>
    public bool IncludeException { get; set; }
}
