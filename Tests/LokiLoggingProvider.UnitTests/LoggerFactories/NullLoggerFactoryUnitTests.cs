namespace LokiLoggingProvider.UnitTests.LoggerFactories
{
    using System;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.LoggerFactories;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class NullLoggerFactoryUnitTests
    {
        public class CreateLogger
        {
            [Fact]
            public void When_CreatingLogger_Expect_LoggerCreated()
            {
                // Arrange
                ILokiLoggerFactory loggerFactory = new LokiLoggingProvider.LoggerFactories.NullLoggerFactory();
                string categoryName = nameof(categoryName);

                // Act
                ILogger logger = loggerFactory.CreateLogger(categoryName);

                // Assert
                Assert.IsType<NullLogger>(logger);
            }

            [Fact]
            public void When_CreatingLoggerWithDisposedLoggerFactory_NoExceptions()
            {
                // Arrange
                ILokiLoggerFactory loggerFactory = new LokiLoggingProvider.LoggerFactories.NullLoggerFactory();
                loggerFactory.Dispose();

                string categoryName = nameof(categoryName);

                // Act
                Exception result = Record.Exception(() => loggerFactory.CreateLogger(categoryName));

                // Assert
                Assert.Null(result);
            }
        }

        public class Dispose
        {
            [Fact]
            public void When_DisposingMoreThanOnce_Expect_NoExceptions()
            {
                // Arrange
                ILokiLoggerFactory loggerFactory = new LokiLoggingProvider.LoggerFactories.NullLoggerFactory();

                // Act
                Exception result = Record.Exception(() =>
                {
                    loggerFactory.Dispose();
                    loggerFactory.Dispose();
                });

                // Assert
                Assert.Null(result);
            }
        }

        public class SetScopeProvider
        {
            [Fact]
            public void When_SettingScopeProvider_Expect_NoExceptions()
            {
                // Arrange
                ILokiLoggerFactory loggerFactory = new LokiLoggingProvider.LoggerFactories.NullLoggerFactory();

                // Act
                Exception result = Record.Exception(() => loggerFactory.SetScopeProvider(NullExternalScopeProvider.Instance));

                // Arrange
                Assert.Null(result);
            }
        }
    }
}
