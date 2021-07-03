namespace LokiLoggingProvider.Formatters
{
    using System;
    using LokiLoggingProvider.Options;

    internal static class FormatterExtensions
    {
        public static ILogEntryFormatter CreateFormatter(this Formatter formatter)
        {
            switch (formatter)
            {
                case Formatter.Json:
                    throw new NotImplementedException();
                case Formatter.Logfmt:
                    return new LogfmtFormatter();
                case Formatter.Simple:
                default:
                    return new SimpleFormatter();
            }
        }
    }
}
