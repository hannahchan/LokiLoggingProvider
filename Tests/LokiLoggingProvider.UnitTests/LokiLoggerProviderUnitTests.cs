namespace LokiLoggingProvider.UnitTests
{
    using System;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class LokiLoggerProviderUnitTests
    {
        public class CreateLogger
        {
            [Fact]
            public void When_CreatingLogger_Expect_LoggerCreated()
            {
                // Arrange
                MockOptionsMonitor options = new MockOptionsMonitor(new LokiLoggerOptions());
                ILoggerProvider loggerProvider = new LokiLoggerProvider(options);

                string categoryName = nameof(categoryName);

                // Act
                ILogger logger = loggerProvider.CreateLogger(categoryName);

                // Assert
                Assert.IsType<NullLogger>(logger);
            }

            [Fact]
            public void When_CreatingLoggerWithDisposedLoggerProvider_Expect_ObjectDisposedException()
            {
                // Arrange
                MockOptionsMonitor options = new MockOptionsMonitor(new LokiLoggerOptions());
                ILoggerProvider loggerProvider = new LokiLoggerProvider(options);
                loggerProvider.Dispose();

                string categoryName = nameof(categoryName);

                // Act
                Exception result = Record.Exception(() => loggerProvider.CreateLogger(categoryName));

                // Assert
                ObjectDisposedException objectDisposedException = Assert.IsType<ObjectDisposedException>(result);
                Assert.Equal("LokiLoggerProvider", objectDisposedException.ObjectName);
            }
        }

        public class Dispose
        {
            [Fact]
            public void When_DisposingMoreThanOnce_Expect_NoExceptions()
            {
                // Arrange
                MockOptionsMonitor options = new MockOptionsMonitor(new LokiLoggerOptions());
                LokiLoggerProvider loggerProvider = new LokiLoggerProvider(options);

                // Act
                Exception result = Record.Exception(() =>
                {
                    loggerProvider.Dispose();
                    loggerProvider.Dispose();
                });

                // Assert
                Assert.Null(result);
            }
        }

        public class SetScopeProvider
        {
            [Theory]
            [InlineData(PushClient.Grpc)]
            [InlineData(PushClient.Http)]
            public void When_SettingScopeProvider_Expect_ScopeProviderSet(PushClient client)
            {
                // Arrange
                MockOptionsMonitor options = new MockOptionsMonitor(new LokiLoggerOptions { Client = client });
                ILoggerProvider loggerProvider = new LokiLoggerProvider(options);

                string categoryName = nameof(categoryName);

                // Act
                ((ISupportExternalScope)loggerProvider).SetScopeProvider(NullExternalScopeProvider.Instance);

                // Assert
                LokiLogger logger = Assert.IsType<LokiLogger>(loggerProvider.CreateLogger(categoryName));
                Assert.IsType<NullExternalScopeProvider>(logger.ScopeProvider);
            }
        }

        public class UpdatingOptions
        {
            [Fact]
            public void When_UpdatingOptions_Expect_UpdatedLogger()
            {
                // Arrange
                LokiLoggerOptions originalOptions = new LokiLoggerOptions { Client = PushClient.None };
                LokiLoggerOptions updatedOptions = new LokiLoggerOptions { Client = PushClient.Grpc };

                MockOptionsMonitor optionsMonitor = new MockOptionsMonitor(originalOptions);
                LokiLoggerProvider loggerProvider = new LokiLoggerProvider(optionsMonitor);

                string categoryName = nameof(categoryName);

                // Act
                ILogger firstLogger = loggerProvider.CreateLogger(categoryName);
                optionsMonitor.Set(updatedOptions);
                ILogger secondLogger = loggerProvider.CreateLogger(categoryName);

                // Assert
                Assert.NotSame(firstLogger, secondLogger);
                Assert.IsType<NullLogger>(firstLogger);
                Assert.IsType<LokiLogger>(secondLogger);
            }
        }

        private sealed class MockDisposable : IDisposable
        {
            public void Dispose()
            {
                // Mock Disposable
            }
        }

        private class MockOptionsMonitor : IOptionsMonitor<LokiLoggerOptions>
        {
            private Action<LokiLoggerOptions, string> listener;

            public MockOptionsMonitor(LokiLoggerOptions currentValue)
            {
                this.CurrentValue = currentValue;
            }

            public LokiLoggerOptions CurrentValue { get; private set; }

            public LokiLoggerOptions Get(string name)
            {
                throw new NotImplementedException();
            }

            public IDisposable OnChange(Action<LokiLoggerOptions, string> listener)
            {
                this.listener = listener;
                return new MockDisposable();
            }

            public void Set(LokiLoggerOptions value)
            {
                this.CurrentValue = value;
                this.listener.Invoke(value, null);
            }
        }
    }
}
