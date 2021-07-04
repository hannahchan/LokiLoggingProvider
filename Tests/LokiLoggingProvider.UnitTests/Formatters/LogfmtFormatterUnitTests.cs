namespace LokiLoggingProvider.Formatters
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class LogfmtFormatterUnitTests
    {
        public class Format
        {
            [Fact]
            public void When_FormattingLogEntry_Expect_Message()
            {
                // Arrange
                LogfmtFormatter formatter = new LogfmtFormatter();
                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "myCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("logLevel=Information category=myCategory eventId=0 message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithException_Expect_Message()
            {
                // Arrange
                LogfmtFormatter formatter = new LogfmtFormatter();
                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "myCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: new Exception("My Exception."),
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"logLevel=Information category=myCategory eventId=0 message=\"My Log Message.\" exception=System.Exception{Environment.NewLine}System.Exception: My Exception.", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithState_Expect_Message()
            {
                // Arrange
                LogfmtFormatter formatter = new LogfmtFormatter();

                Dictionary<string, object> state = new Dictionary<string, object>
                {
                    { "key1", 123 },
                    { "key2", 123.456 },
                    { "key3", true },
                    { "key4", "value" },
                    { "key5", string.Empty },
                    { "key6", "   " },
                };

                LogEntry<Dictionary<string, object>> logEntry = new LogEntry<Dictionary<string, object>>(
                    logLevel: LogLevel.Error,
                    category: default,
                    eventId: default,
                    state: state,
                    exception: null,
                    formatter: (state, exception) => "My Log Message.");

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("logLevel=Error category=\"\" eventId=0 message=\"My Log Message.\" key1=123 key2=123.456 key3=True key4=value key5=\"\" key6=\"   \"", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithDuplicateKeys_Expect_Message()
            {
                // Arrange
                LogfmtFormatter formatter = new LogfmtFormatter();

                Dictionary<string, object> state = new Dictionary<string, object>
                {
                    { "logLevel", "abc" },
                    { "category", "xyz" },
                    { "eventId", 123 },
                    { "message", "Another message." },
                };

                LogEntry<Dictionary<string, object>> logEntry = new LogEntry<Dictionary<string, object>>(
                    logLevel: LogLevel.Error,
                    category: default,
                    eventId: default,
                    state: state,
                    exception: null,
                    formatter: (state, exception) => "My Log Message.");

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("logLevel=Error category=\"\" eventId=0 message=\"My Log Message.\" logLevel=abc category=xyz eventId=123 message=\"Another message.\"", result);
            }
        }
    }
}
