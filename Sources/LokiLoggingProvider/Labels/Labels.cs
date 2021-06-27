namespace LokiLoggingProvider.Labels
{
    internal static class Labels
    {
        public const string Job = "job";

        public const string Instance = "instance";

        public const string CategoryName = "categoryName";

        public const string LogLevel = "logLevel";

        public const string EventId = "eventId";

        public const string Exception = "exception";

        public static readonly string[] All = { Job, Instance, CategoryName, LogLevel, EventId, Exception };
    }
}
