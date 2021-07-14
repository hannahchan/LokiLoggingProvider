namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Collections.Generic;

    internal class LogValues : Dictionary<string, object?>
    {
        public LogValues()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void SetLogLevel(object? value)
        {
            this["LogLevel"] = value;
        }

        public void SetCategory(object? value)
        {
            this["Category"] = value;
        }

        public void SetEventId(object? value)
        {
            this["EventId"] = value;
        }

        public void SetMessage(object? value)
        {
            this["Message"] = value;
        }

        public void SetState(object? value)
        {
            this["State"] = value;
        }

        public void SetScopes(object? value)
        {
            this["Scopes"] = value;
        }

        public void SetException(object? value)
        {
            this["Exception"] = value;
        }

        public void SetExceptionDetails(object? value)
        {
            this["ExceptionDetails"] = value;
        }
    }
}
