namespace LokiLoggingProvider.Logger
{
    using System;

    internal struct LokiLogEntry
    {
        public readonly DateTime Timestamp;

        public readonly LabelValues Labels;

        public readonly string Message;

        public LokiLogEntry(DateTime timestamp, LabelValues labels, string message)
        {
            this.Timestamp = timestamp;
            this.Labels = labels;
            this.Message = message;
        }
    }
}
