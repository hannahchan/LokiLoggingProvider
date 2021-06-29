namespace LokiLoggingProvider.UnitTests.Logger
{
    using System;
    using System.Collections.Generic;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using LokiLoggingProvider.PushClients;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class LokiLoggerUnitTests
    {
        public class BeginScope
        {
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
                LokiLogMessageEntryProcessor processor = new LokiLogMessageEntryProcessor(client);
                LokiLoggerOptions options = new LokiLoggerOptions();

                LokiLogger logger = new LokiLogger(
                    categoryName,
                    processor,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.FormatterOptions);

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
            private readonly IList<LokiLogMessageEntry> receivedLogMessageEntries = new List<LokiLogMessageEntry>();

            public void Dispose()
            {
                this.receivedLogMessageEntries.Clear();
            }

            public void Push(LokiLogMessageEntry entry)
            {
                this.receivedLogMessageEntries.Add(entry);
            }

            public IEnumerable<LokiLogMessageEntry> GetReceivedLogMessageEntries()
            {
                return this.receivedLogMessageEntries;
            }
        }
    }
}
