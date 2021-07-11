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

        public object? LogLevel
        {
            get => this[Key.LogLevel];
            set => this[Key.LogLevel] = value;
        }

        public object? Category
        {
            get => this[Key.Category];
            set => this[Key.Category] = value;
        }

        public object? EventId
        {
            get => this[Key.EventId];
            set => this[Key.EventId] = value;
        }

        public object? Message
        {
            get => this[Key.Message];
            set => this[Key.Message] = value;
        }

        public object? State
        {
            get => this[Key.State];
            set => this[Key.State] = value;
        }

        public object? Exception
        {
            get => this[Key.Exception];
            set => this[Key.Exception] = value;
        }

        public object? ExceptionDetails
        {
            get => this[Key.ExceptionDetails];
            set => this[Key.ExceptionDetails] = value;
        }

        private static class Key
        {
            public const string LogLevel = "LogLevel";

            public const string Category = "Category";

            public const string EventId = "EventId";

            public const string Message = "Message";

            public const string State = "State";

            public const string Exception = "Exception";

            public const string ExceptionDetails = "ExceptionDetails";
        }
    }
}
