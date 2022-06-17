namespace LokiLoggingProvider.Options;

public class HttpOptions
{
    /// <summary>The destination to send logs to. Defaults to "http://localhost:3100".</summary>
    public string Address { get; set; } = "http://localhost:3100";

    /// <summary>The user used to authenticate to a reverse proxy or Grafana Cloud.</summary>
    public string User { get; set; } = string.Empty;

    /// <summary>The password used to authenticate to a reverse proxy or Grafana Cloud.</summary>
    public string Password { get; set; } = string.Empty;
}
