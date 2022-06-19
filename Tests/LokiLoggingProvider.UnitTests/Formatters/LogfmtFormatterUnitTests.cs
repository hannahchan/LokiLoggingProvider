namespace LokiLoggingProvider.UnitTests.Formatters;

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
        public void When_FormattingLogEntry_Expect_Message()
        {
            // Arrange
            LogfmtFormatterOptions options = new();
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingCategory_Expect_MessageWithCategory()
        {
            // Arrange
            LogfmtFormatterOptions options = new()
            {
                IncludeCategory = true,
            };

            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Information Category=MyCategory Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingEventId_Expect_MessageWithEventId()
        {
            // Arrange
            LogfmtFormatterOptions options = new()
            {
                IncludeEventId = true,
            };

            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Information EventId=0 Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithNullFormatter_Expect_MessageWithEmptyString()
        {
            // Arrange
            LogfmtFormatterOptions options = new();
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: null);

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Information Message=\"\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithException_Expect_MessageWithExceptionPrinted()
        {
            // Arrange
            LogfmtFormatterOptions options = new();
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: new Exception("My Exception."),
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal($"Level=Information Message=\"My Log Message.\" Exception=System.Exception{Environment.NewLine}System.Exception: My Exception.", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithException_Expect_MessageWithExceptionNotPrinted()
        {
            // Arrange
            LogfmtFormatterOptions options = new()
            {
                PrintExceptions = false,
            };

            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: new Exception("My Exception."),
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal($"Level=Information Message=\"My Log Message.\" Exception=System.Exception", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithEnumerableState_Expect_MessageWithStateKeyValues()
        {
            // Arrange
            LogfmtFormatterOptions options = new();
            LogfmtFormatter formatter = new(options);

            Dictionary<string, object> state = new()
            {
                { "key1", 123 },
                { "key2", 123.456 },
                { "key3", true },
                { "key4", "value" },
                { "key 5", string.Empty },
                { "key   6", "   " },
                { "key7", null },
            };

            LogEntry<Dictionary<string, object>> logEntry = new(
                logLevel: LogLevel.Error,
                category: default,
                eventId: default,
                state: state,
                exception: null,
                formatter: (state, exception) => "My Log Message.");

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Error Message=\"My Log Message.\" key1=123 key2=123.456 key3=True key4=value key5=\"\" key6=\"   \" key7=\"\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithEnumerableState_Expect_MessageWithNoOverriddenKeys()
        {
            // Arrange
            LogfmtFormatterOptions options = new();
            LogfmtFormatter formatter = new(options);

            Dictionary<string, object> state = new()
            {
                { "level", "abc" },
                { "Category", "Nothing overridden here." },
                { "EventId", 123 },
                { "message", "Another message." },
            };

            LogEntry<Dictionary<string, object>> logEntry = new(
                logLevel: LogLevel.Error,
                category: default,
                eventId: default,
                state: state,
                exception: null,
                formatter: (state, exception) => "My Log Message.");

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Error Message=\"My Log Message.\" Category=\"Nothing overridden here.\" EventId=123", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithNonEnumerableState_Expect_MessageWithoutState()
        {
            // Arrange
            LogfmtFormatterOptions options = new();
            LogfmtFormatter formatter = new(options);

            string state = "My State.";

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Error,
                category: default,
                eventId: default,
                state: state,
                exception: null,
                formatter: (state, exception) => "My Log Message.");

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Error Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingScopeWithScopeProvider_Expect_MessageWithScopes()
        {
            // Arrange
            LogfmtFormatterOptions options = new() { IncludeScopes = true };
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            IExternalScopeProvider scopeProvider = new MockScopeProvider(new List<object>
            {
                "This string should not appear.",
                new
                {
                    Name = "This object should not appear",
                    Value = 123,
                },
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("SomeKey", "abc"),
                    new KeyValuePair<string, object>("SomeKey", "Duplicate key should not appear"),
                },
                new Dictionary<string, object>
                {
                    ["Key1"] = "abc",
                    ["Key2"] = 123,
                    ["SomeKey"] = "Duplicate key should not appear",
                },
            });

            // Act
            string result = formatter.Format(logEntry, scopeProvider);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\" SomeKey=abc Key1=abc Key2=123", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingScopeWithScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            LogfmtFormatterOptions options = new() { IncludeScopes = true };
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            IExternalScopeProvider scopeProvider = new MockScopeProvider(new List<object>());

            // Act
            string result = formatter.Format(logEntry, scopeProvider);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingScopeWithNullScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            LogfmtFormatterOptions options = new() { IncludeScopes = true };
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry, null);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryNotIncludingScopeWithScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            LogfmtFormatterOptions options = new() { IncludeScopes = false };
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            IExternalScopeProvider scopeProvider = new MockScopeProvider(new List<object> { "A String Scope" });

            // Act
            string result = formatter.Format(logEntry, scopeProvider);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\"", result);
        }

        [Fact]
        public void When_FormattingLogEntryNotIncludingScopeWithNullScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            LogfmtFormatterOptions options = new() { IncludeScopes = false };
            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Act
            string result = formatter.Format(logEntry, null);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\"", result);
        }
    }

    [Collection(TestCollection.Activity)]
    public class FormatWithActivityTracking
    {
        [Fact]
        public void When_FormattingLogEntryIncludingActivityTracking_Expect_MessageWithActivityTracking()
        {
            // Arrange
            LogfmtFormatterOptions options = new()
            {
                IncludeActivityTracking = true,
            };

            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            using Activity parentActivity = new(nameof(parentActivity));
            using Activity childActivity = new(nameof(childActivity));

            // Act
            parentActivity.Start();
            childActivity.Start();

            string result = formatter.Format(logEntry);

            // Assert
            Assert.StartsWith("Level=Information Message=\"My Log Message.\" ", result);
            Assert.EndsWith($" SpanId={childActivity.GetSpanId()} TraceId={childActivity.GetTraceId()} ParentId={parentActivity.GetSpanId()}", result);
        }

        [Fact]
        public void When_FormattingLogEntryNotIncludingActivityTracking_Expect_MessageWithNoActivityTracking()
        {
            // Arrange
            LogfmtFormatterOptions options = new()
            {
                IncludeActivityTracking = false,
            };

            LogfmtFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            using Activity parentActivity = new(nameof(parentActivity));
            using Activity childActivity = new(nameof(childActivity));

            // Act
            parentActivity.Start();
            childActivity.Start();

            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("Level=Information Message=\"My Log Message.\"", result);
        }
    }

    private class MockScopeProvider : IExternalScopeProvider
    {
        private readonly List<object> scopes;

        public MockScopeProvider(List<object> scopes)
        {
            this.scopes = scopes;
        }

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
            foreach (object scope in this.scopes)
            {
                callback(scope, state);
            }
        }

        public IDisposable Push(object state)
        {
            throw new NotImplementedException();
        }
    }
}
