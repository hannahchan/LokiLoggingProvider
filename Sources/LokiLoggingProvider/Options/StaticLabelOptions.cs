namespace LokiLoggingProvider.Options;

using System.Collections.Generic;

public class StaticLabelOptions
{
    /// <summary>The value to set the label 'job' to. Recommended to match the value that is used in Prometheus.</summary>
    public string JobName { get; set; } = string.Empty;

    /// <summary>Includes a label named 'instance' with the value set to the hostname. Defaults to 'true'.</summary>
    public bool IncludeInstanceLabel { get; set; } = true;

    /// <summary>Additional or custom static labels and their values to add to all log streams.</summary>
    public Dictionary<string, object?> AdditionalStaticLabels { get; set; } = new Dictionary<string, object?>();
}
