namespace LokiLoggingProvider.Options
{
    public class DynamicLabelOptions
    {
        public bool IncludeCategoryName { get; set; }

        public bool IncludeLogLevel { get; set; }

        public bool IncludeEventId { get; set; }

        public bool IncludeException { get; set; }
    }
}
