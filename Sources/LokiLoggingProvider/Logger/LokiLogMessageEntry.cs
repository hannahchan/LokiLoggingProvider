namespace LokiLoggingProvider.Logger
{
    using System;
    using System.Collections.Generic;

    internal struct LokiLogMessageEntry
    {
        public readonly DateTime Timestamp;

        public readonly IReadOnlyDictionary<string, string> Labels;

        public readonly string Message;

        public LokiLogMessageEntry(DateTime timestamp, IReadOnlyDictionary<string, string> labels, string message)
        {
            this.Timestamp = timestamp;
            this.Labels = labels;
            this.Message = message;
        }
    }
}
