namespace LokiLoggingProvider.Formatters;

using System;
using System.Diagnostics;
using LokiLoggingProvider.Extensions;
using LokiLoggingProvider.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

internal class SimpleFormatter : ILogEntryFormatter
{
    private readonly SimpleFormatterOptions formatterOptions;

    public SimpleFormatter(SimpleFormatterOptions formatterOptions)
    {
        this.formatterOptions = formatterOptions;
    }

    public string Format<TState>(LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider = null)
    {
        string message = $"[{GetLogLevelString(logEntry.LogLevel)}] ";

        if (this.formatterOptions.IncludeActivityTracking && Activity.Current is Activity activity)
        {
            message += $"{activity.GetTraceId()} - ";
        }

        message += logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception) ?? "Something happened.";

        if (logEntry.Exception != null)
        {
            message += Environment.NewLine + logEntry.Exception.ToString();
        }

        return message;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "TRCE",
            LogLevel.Debug => "DBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "EROR",
            LogLevel.Critical => "CRIT",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    }
}
