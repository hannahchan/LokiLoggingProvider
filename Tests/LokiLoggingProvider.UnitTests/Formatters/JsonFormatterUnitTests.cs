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

public class JsonFormatterUnitTests
{
    public class Format
    {
        [Fact]
        public void When_FormattingLogEntry_Expect_Message()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntry_Expect_IndentedMessage()
        {
            // Arrange
            JsonFormatterOptions options = new() { WriteIndented = true };
            JsonFormatter formatter = new(options);

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
            Assert.Equal($"{{{Environment.NewLine}  \"Level\": \"Information\",{Environment.NewLine}  \"Message\": \"My Log Message.\"{Environment.NewLine}}}", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingCategory_Expect_MessageWithCategory()
        {
            // Arrange
            JsonFormatterOptions options = new()
            {
                IncludeCategory = true,
            };

            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Category\":\"MyCategory\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingEventId_Expect_MessageWithEventId()
        {
            // Arrange
            JsonFormatterOptions options = new()
            {
                IncludeEventId = true,
            };

            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"EventId\":0,\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithNullFormatter_Expect_MessageWithNullValue()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":null}", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithEnumerableState_Expect_MessageWithStateDictionary()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Error\",\"Message\":\"My Log Message.\",\"State\":{\"key1\":123,\"key2\":123.456,\"key3\":true,\"key4\":\"value\",\"key 5\":\"\",\"key   6\":\"   \",\"key7\":null}}", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithEmptyEnumerableState_Expect_MessageWithoutState()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

            Dictionary<string, object> state = new();

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
            Assert.Equal("{\"Level\":\"Error\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithEnumerableState_Expect_MessageWithStateKeyValuePairs()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

            List<KeyValuePair<string, object>> state = new()
            {
                new KeyValuePair<string, object>("DuplicateKey", "abc"),
                new KeyValuePair<string, object>("DuplicateKey", 123),
            };

            LogEntry<List<KeyValuePair<string, object>>> logEntry = new(
                logLevel: LogLevel.Error,
                category: default,
                eventId: default,
                state: state,
                exception: null,
                formatter: (state, exception) => "My Log Message.");

            // Act
            string result = formatter.Format(logEntry);

            // Assert
            Assert.Equal("{\"Level\":\"Error\",\"Message\":\"My Log Message.\",\"State\":[{\"Key\":\"DuplicateKey\",\"Value\":\"abc\"},{\"Key\":\"DuplicateKey\",\"Value\":123}]}", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithNonEnumerableState_Expect_MessageWithoutState()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Error\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryWithException_Expect_MessageWithException()
        {
            // Arrange
            JsonFormatterOptions options = new();
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\",\"Exception\":\"System.Exception\",\"ExceptionDetails\":\"System.Exception: My Exception.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingScopeWithScopeProvider_Expect_MessageWithScopes()
        {
            // Arrange
            JsonFormatterOptions options = new() { IncludeScopes = true };
            JsonFormatter formatter = new(options);

            LogEntry<string> logEntry = new(
                logLevel: LogLevel.Information,
                category: "MyCategory",
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            IExternalScopeProvider scopeProvider = new MockScopeProvider(new List<object>
            {
                "A String",
                new
                {
                    Name = "An Object",
                    Value = 123,
                },
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("DuplicateKey", "abc"),
                    new KeyValuePair<string, object>("DuplicateKey", 123),
                },
                new Dictionary<string, object>
                {
                    ["Key1"] = "abc",
                    ["Key2"] = 123,
                },
            });

            // Act
            string result = formatter.Format(logEntry, scopeProvider);

            // Assert
            Assert.StartsWith("{\"Level\":\"Information\",\"Message\":\"My Log Message.\",\"Scopes\":[\"A String\",{\"Name\":\"An Object\",\"Value\":123},", result);
            Assert.EndsWith("[{\"Key\":\"DuplicateKey\",\"Value\":\"abc\"},{\"Key\":\"DuplicateKey\",\"Value\":123}],{\"Key1\":\"abc\",\"Key2\":123}]}", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingScopeWithScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            JsonFormatterOptions options = new() { IncludeScopes = true };
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryIncludingScopeWithNullScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            JsonFormatterOptions options = new() { IncludeScopes = true };
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryNotIncludingScopeWithScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            JsonFormatterOptions options = new() { IncludeScopes = false };
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\"}", result);
        }

        [Fact]
        public void When_FormattingLogEntryNotIncludingScopeWithNullScopeProvider_Expect_MessageWithoutScopes()
        {
            // Arrange
            JsonFormatterOptions options = new() { IncludeScopes = false };
            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\"}", result);
        }
    }

    [Collection(TestCollection.Activity)]
    public class FormatWithActivityTracking
    {
        [Fact]
        public void When_FormattingLogEntryIncludingActivityTracking_Expect_MessageWithActivityTracking()
        {
            // Arrange
            JsonFormatterOptions options = new()
            {
                IncludeActivityTracking = true,
            };

            JsonFormatter formatter = new(options);

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
            Assert.StartsWith("{\"Level\":\"Information\",\"Message\":\"My Log Message.\",", result);
            Assert.EndsWith($",\"SpanId\":\"{childActivity.GetSpanId()}\",\"TraceId\":\"{childActivity.GetTraceId()}\",\"ParentId\":\"{parentActivity.GetSpanId()}\"}}", result);
        }

        [Fact]
        public void When_FormattingLogEntryNotIncludingActivityTracking_Expect_MessageWithNoActivityTracking()
        {
            // Arrange
            JsonFormatterOptions options = new()
            {
                IncludeActivityTracking = false,
            };

            JsonFormatter formatter = new(options);

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
            Assert.Equal("{\"Level\":\"Information\",\"Message\":\"My Log Message.\"}", result);
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
