namespace LokiLoggingProvider.UnitTests.Logger
{
    using System;
    using System.Collections.Generic;
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
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
                MockLokiPushClient client = new MockLokiPushClient();

                string categoryName = nameof(categoryName);
                ILogEntryFormatter formatter = new SimpleFormatter();
                LokiLogEntryProcessor processor = new LokiLogEntryProcessor(client);
                LokiLoggerOptions options = new LokiLoggerOptions();
                MockScopeProvider scopeProvider = new MockScopeProvider();

                LokiLogger logger = new LokiLogger(
                    categoryName,
                    formatter,
                    processor,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions)
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
                MockLokiPushClient client = new MockLokiPushClient();

                string categoryName = nameof(categoryName);
                ILogEntryFormatter formatter = new SimpleFormatter();
                LokiLogEntryProcessor processor = new LokiLogEntryProcessor(client);
                LokiLoggerOptions options = new LokiLoggerOptions();

                LokiLogger logger = new LokiLogger(
                    categoryName,
                    formatter,
                    processor,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions);

                // Act
                bool result = logger.IsEnabled(logLevel);

                // Assert
                Assert.Equal(isEnabled, result);
            }
        }

        public class Log
        {
        }

        private sealed class MockLokiPushClient : ILokiPushClient
        {
            private readonly IList<LokiLogEntry> receivedLogMessageEntries = new List<LokiLogEntry>();

            public void Dispose()
            {
                this.receivedLogMessageEntries.Clear();
            }

            public void Push(LokiLogEntry entry)
            {
                this.receivedLogMessageEntries.Add(entry);
            }

            public IEnumerable<LokiLogEntry> GetReceivedLogEntries()
            {
                return this.receivedLogMessageEntries;
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
}
