namespace LokiLoggingProvider.UnitTests.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class LogfmtFormatterUnitTests
    {
        public class Format
        {
            [Fact]
            public void When_FormattingLogEntry_Expect_DefaultMessage()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions();
                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Information Message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryIncludingCategory_Expect_MessageWithCategory()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    IncludeCategory = true,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Information Category=MyCategory Message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryIncludingEventId_Expect_MessageWithEventId()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    IncludeEventId = true,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Information EventId=0 Message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithException_Expect_MessageWithExceptionPrinted()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions();
                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: new Exception("My Exception."),
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"LogLevel=Information Message=\"My Log Message.\" Exception=System.Exception{Environment.NewLine}System.Exception: My Exception.", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithException_Expect_MessageWithExceptionNotPrinted()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    PrintExceptions = false,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: new Exception("My Exception."),
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal($"LogLevel=Information Message=\"My Log Message.\" Exception=System.Exception", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithEnumerableState_Expect_MessageWithStateKeyValues()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions();
                LogfmtFormatter formatter = new LogfmtFormatter(options);

                Dictionary<string, object> state = new Dictionary<string, object>
                {
                    { "key1", 123 },
                    { "key2", 123.456 },
                    { "key3", true },
                    { "key4", "value" },
                    { "key 5", string.Empty },
                    { "key   6", "   " },
                    { "key7", null },
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
                Assert.Equal("LogLevel=Error Message=\"My Log Message.\" key1=123 key2=123.456 key3=True key4=value key5=\"\" key6=\"   \" key7=\"\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithEnumerableState_Expect_MessageWithNoOverriddenKeys()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions();
                LogfmtFormatter formatter = new LogfmtFormatter(options);

                Dictionary<string, object> state = new Dictionary<string, object>
                {
                    { "logLevel", "abc" },
                    { "Category", "Nothing overridden here." },
                    { "EventId", 123 },
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
                Assert.Equal("LogLevel=Error Message=\"My Log Message.\" Category=\"Nothing overridden here.\" EventId=123", result);
            }

            [Fact]
            public void When_FormattingLogEntryWithNonEnumerableState_Expect_MessageWithoutState()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions();
                LogfmtFormatter formatter = new LogfmtFormatter(options);

                string state = "My State.";

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Error,
                    category: default,
                    eventId: default,
                    state: state,
                    exception: null,
                    formatter: (state, exception) => "My Log Message.");

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Error Message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryIncludingActivityTrackingWithActivity_Expect_MessageWithActivityTracking()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    IncludeActivityTracking = true,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                using Activity parentActivity = new Activity(nameof(parentActivity));
                using Activity childActivity = new Activity(nameof(childActivity));

                // Act
                parentActivity.Start();
                childActivity.Start();

                string result = formatter.Format(logEntry);

                // Assert
                Assert.StartsWith("LogLevel=Information Message=\"My Log Message.\" ", result);
                Assert.EndsWith($" SpanId={childActivity.GetSpanId()} TraceId={childActivity.GetTraceId()} ParentId={parentActivity.GetSpanId()}", result);
            }

            [Fact]
            public void When_FormattingLogEntryIncludingActivityTrackingWithNullActivity_Expect_MessageWithNoActivityTracking()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    IncludeActivityTracking = true,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Information Message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryNotIncludingActivityTrackingWithActivity_Expect_MessageWithNoActivityTracking()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    IncludeActivityTracking = false,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                using Activity parentActivity = new Activity(nameof(parentActivity));
                using Activity childActivity = new Activity(nameof(childActivity));

                // Act
                parentActivity.Start();
                childActivity.Start();

                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Information Message=\"My Log Message.\"", result);
            }

            [Fact]
            public void When_FormattingLogEntryNotIncludingActivityTrackingWithNullActivity_Expect_MessageWithNoActivityTracking()
            {
                // Arrange
                LogfmtFormatterOptions options = new LogfmtFormatterOptions
                {
                    IncludeActivityTracking = false,
                };

                LogfmtFormatter formatter = new LogfmtFormatter(options);

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Information,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                string result = formatter.Format(logEntry);

                // Assert
                Assert.Equal("LogLevel=Information Message=\"My Log Message.\"", result);
            }
        }
    }
}
