namespace LokiLoggingProvider.Logger;

using System;

internal record LokiLogEntry(DateTime Timestamp, LabelValues Labels, string Message);
