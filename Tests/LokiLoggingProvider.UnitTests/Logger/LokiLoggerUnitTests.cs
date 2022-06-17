namespace LokiLoggingProvider.UnitTests.Logger;

using System;
using System.Collections.Generic;
using LokiLoggingProvider.Formatters;
using LokiLoggingProvider.Logger;
using LokiLoggingProvider.Options;
using Microsoft.Extensions.Logging;
using Xunit;

public class LokiLoggerUnitTests
{
    public class BeginScope
    {
        [Fact]
        public void When_BeginningScope_Expect_Scope()
        {
            // Arrange
            string categoryName = nameof(categoryName);
            ILogEntryFormatter formatter = new SimpleFormatter(new SimpleFormatterOptions());
            ILokiLogEntryProcessor processor = new MockLogEntryProcessor();
            LokiLoggerOptions options = new();
            MockScopeProvider scopeProvider = new();

            LokiLogger logger = new(
                categoryName,
                formatter,
                processor,
                options.StaticLabels,
                options.DynamicLabels)
            {
                ScopeProvider = scopeProvider,
            };

            // Act
            IDisposable disposable1 = logger.BeginScope("My Outer Scope.");
            IDisposable disposable2 = logger.BeginScope("My Inner Scope.");

            Exception result = Record.Exception(() =>
            {
                disposable1.Dispose();
                disposable2.Dispose();
            });

            // Assert
            Assert.Collection(
                scopeProvider.States,
                state =>
                {
                    string @string = Assert.IsType<string>(state);
                    Assert.Equal("My Outer Scope.", @string);
                },
                state =>
                {
                    string @string = Assert.IsType<string>(state);
                    Assert.Equal("My Inner Scope.", @string);
                });

            Assert.Null(result);
        }
    }

    public class IsEnabled
    {
        [Theory]
        [InlineData(LogLevel.Trace, true)]
        [InlineData(LogLevel.Debug, true)]
        [InlineData(LogLevel.Information, true)]
        [InlineData(LogLevel.Warning, true)]
        [InlineData(LogLevel.Error, true)]
        [InlineData(LogLevel.Critical, true)]
        [InlineData(LogLevel.None, false)]
        public void When_LogLevelIsEnabled_Expect_IsEnabled(LogLevel logLevel, bool isEnabled)
        {
            // Arrange
            string categoryName = nameof(categoryName);
            ILogEntryFormatter formatter = new SimpleFormatter(new SimpleFormatterOptions());
            ILokiLogEntryProcessor processor = new MockLogEntryProcessor();
            LokiLoggerOptions options = new();

            LokiLogger logger = new(
                categoryName,
                formatter,
                processor,
                options.StaticLabels,
                options.DynamicLabels);

            // Act
            bool result = logger.IsEnabled(logLevel);

            // Assert
            Assert.Equal(isEnabled, result);
        }
    }

    public class Log
    {
        [Fact]
        public void When_LoggingEntry_Expect_Logged()
        {
            // Arrange
            string categoryName = nameof(categoryName);
            ILogEntryFormatter formatter = new SimpleFormatter(new SimpleFormatterOptions());
            MockLogEntryProcessor processor = new();
            LokiLoggerOptions options = new();

            LokiLogger logger = new(
                categoryName,
                formatter,
                processor,
                options.StaticLabels,
                options.DynamicLabels);

            // Act
            logger.Log(
                logLevel: LogLevel.Information,
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Assert
            Assert.Collection(processor.LogEntries, logEntry => Assert.Equal("[INFO] My Log Message.", logEntry.Message));
        }

        [Fact]
        public void When_LoggingEntry_Expect_NotLogged()
        {
            // Arrange
            string categoryName = nameof(categoryName);
            ILogEntryFormatter formatter = new SimpleFormatter(new SimpleFormatterOptions());
            MockLogEntryProcessor processor = new();
            LokiLoggerOptions options = new();

            LokiLogger logger = new(
                categoryName,
                formatter,
                processor,
                options.StaticLabels,
                options.DynamicLabels);

            // Act
            logger.Log(
                logLevel: LogLevel.None,
                eventId: default,
                state: "My Log Message.",
                exception: null,
                formatter: (state, exception) => state.ToString());

            // Assert
            Assert.Empty(processor.LogEntries);
        }
    }

    private sealed class MockLogEntryProcessor : ILokiLogEntryProcessor
    {
        public List<LokiLogEntry> LogEntries { get; } = new List<LokiLogEntry>();

        public void Dispose()
        {
            // Do nothing
        }

        public void EnqueueMessage(LokiLogEntry message)
        {
            this.LogEntries.Add(message);
        }
    }

    private class MockScopeProvider : IExternalScopeProvider
    {
        public List<object> States { get; } = new List<object>();

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
            // Do nothing
        }

        public IDisposable Push(object state)
        {
            this.States.Add(state);
            return NullScope.Instance;
        }
    }
}
