namespace LokiLoggingProvider.UnitTests.Formatters
{
    using System;
    using System.Diagnostics;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class SimpleFormatterUnitTests
    {
        public class Format
        {
            [Theory]
            [InlineData(LogLevel.Trace, "TRCE")]
            [InlineData(LogLevel.Debug, "DBUG")]
            [InlineData(LogLevel.Information, "INFO")]
            [InlineData(LogLevel.Warning, "WARN")]
            [InlineData(LogLevel.Error, "EROR")]
            [InlineData(LogLevel.Critical, "CRIT")]
            public void When_FormattingLogEntry_Expect_Message(LogLevel logLevel, string logLevelString)
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions();
                SimpleFormatter formatter = new SimpleFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: logLevel,
                    category: default,
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"[{logLevelString}] My Log Message.", result);
            }

            [Fact]
            public void When_FormattingLogEntry_Expect_ArgumentOutOfRangeException()
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions();
                SimpleFormatter formatter = new SimpleFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.None,
                    category: default,
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                Exception result = Record.Exception(() => formatter.Format(logEntry));

                // Assert
                ArgumentOutOfRangeException argumentOutOfRangeException = Assert.IsType<ArgumentOutOfRangeException>(result);
                Assert.Equal("logLevel", argumentOutOfRangeException.ParamName);
            }

            [Fact]
            public void When_FormattingLogEntryIncludingActivityTrackingWithActivity_Expect_MessageWithActivityTracking()
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions
                {
                    IncludeActivityTracking = true,
                };

                SimpleFormatter formatter = new SimpleFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: default,
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                using Activity activity = new Activity(nameof(activity));

                // Act
                activity.Start();

                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"[INFO] {activity.GetTraceId()} - My Log Message.", result);
            }

            [Fact]
            public void When_FormattingLogEntryIncludingActivityTrackingWithNullActivity_Expect_MessageWithNoActivityTracking()
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions
                {
                    IncludeActivityTracking = true,
                };

                SimpleFormatter formatter = new SimpleFormatter(options);

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
                Assert.Equal($"[INFO] My Log Message.", result);
            }

            [Fact]
            public void When_FormattingLogEntryNotIncludingActivityTrackingWithActivity_Expect_MessageWithNoActivityTracking()
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions
                {
                    IncludeActivityTracking = false,
                };

                SimpleFormatter formatter = new SimpleFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: default,
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                using Activity activity = new Activity(nameof(activity));

                // Act
                activity.Start();

                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"[INFO] My Log Message.", result);
            }

            [Fact]
            public void When_FormattingLogEntryNotIncludingActivityTrackingWithNullActivity_Expect_MessageWithNoActivityTracking()
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions
                {
                    IncludeActivityTracking = false,
                };

                SimpleFormatter formatter = new SimpleFormatter(options);

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
                Assert.Equal($"[INFO] My Log Message.", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithException_Expect_Message()
            {
                // Arrange
                SimpleFormatterOptions options = new SimpleFormatterOptions();
                SimpleFormatter formatter = new SimpleFormatter(options);

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
                Assert.Equal($"[EROR] My Log Message.{Environment.NewLine}System.Exception: My Exception.", result);
            }
        }
    }
}
