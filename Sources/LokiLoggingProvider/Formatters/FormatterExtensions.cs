namespace LokiLoggingProvider.Formatters
{
    using System;
    using LokiLoggingProvider.Options;

    internal static class FormatterExtensions
    {
        public static ILogEntryFormatter CreateFormatter(this Formatter formatter)
        {
            return formatter switch
            {
                Formatter.Json => throw new NotImplementedException(),
                Formatter.Logfmt => new LogfmtFormatter(),
                _ => new SimpleFormatter(),
            };
        }
    }
}
