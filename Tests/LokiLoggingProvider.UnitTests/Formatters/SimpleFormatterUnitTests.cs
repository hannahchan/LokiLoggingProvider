namespace LokiLoggingProvider.Formatters
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class SimpleFormatterUnitTests
    {
        public class Format
        {
            [Fact]
            public void When_FormattingLogEntry_Expect_Message()
            {
                // Arrange
                SimpleFormatter formatter = new SimpleFormatter();
                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: default,
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("[Information] My Log Message.", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithException_Expect_Message()
            {
                // Arrange
                SimpleFormatter formatter = new SimpleFormatter();
                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Error,
                    category: default,
                    eventId: default,
                    state: "My Log Message.",
                    exception: new Exception("My Exception."),
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"[Error] My Log Message.{Environment.NewLine}System.Exception: My Exception.", result);
            }
        }
    }
}
