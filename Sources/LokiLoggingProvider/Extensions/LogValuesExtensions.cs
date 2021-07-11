namespace LokiLoggingProvider.Extensions
{
    using System.Diagnostics;
    using LokiLoggingProvider.Formatters;

    internal static class LogValuesExtensions
    {
        public static LogValues AddActivityTracking(this LogValues logValues)
        {
            if (Activity.Current is Activity activity)
            {
                logValues.TryAdd("SpanId", activity.GetSpanId());
                logValues.TryAdd("TraceId", activity.GetTraceId());
                logValues.TryAdd("ParentId", activity.GetParentId());
            }

            return logValues;
        }
    }
}
