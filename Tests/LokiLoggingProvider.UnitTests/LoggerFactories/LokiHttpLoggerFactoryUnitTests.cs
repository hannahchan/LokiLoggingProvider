namespace LokiLoggingProvider.UnitTests.LoggerFactories
{
    using System;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.LoggerFactories;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class LokiHttpLoggerFactoryUnitTests
    {
        public class CreateLogger
        {
            [Fact]
            public void When_CreatingLogger_Expect_LoggerCreated()
            {
                // Arrange
                LokiLoggerOptions options = new LokiLoggerOptions();
                ILokiLoggerFactory loggerFactory = new LokiHttpLoggerFactory(
                    options.HttpOptions,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.FormatterOptions);

                string categoryName = nameof(categoryName);

                // Act
                ILogger logger = loggerFactory.CreateLogger(categoryName);

                // Assert
                Assert.IsType<LokiLogger>(logger);
            }

            [Fact]
            public void When_CreatingLoggerWithDisposedLoggerFactory_Expect_ObjectDisposedException()
            {
                // Arrange
                LokiLoggerOptions options = new LokiLoggerOptions();
                ILokiLoggerFactory loggerFactory = new LokiHttpLoggerFactory(
                    options.HttpOptions,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.FormatterOptions);

                loggerFactory.Dispose();

                string categoryName = nameof(categoryName);

                // Act
                Exception result = Record.Exception(() => loggerFactory.CreateLogger(categoryName));

                // Assert
                ObjectDisposedException objectDisposedException = Assert.IsType<ObjectDisposedException>(result);
                Assert.Equal(nameof(LokiHttpLoggerFactory), objectDisposedException.ObjectName);
            }
        }

        public class Dispose
        {
            [Fact]
            public void When_DisposingMoreThanOnce_Expect_NoExceptions()
            {
                // Arrange
                LokiLoggerOptions options = new LokiLoggerOptions();
                ILokiLoggerFactory loggerFactory = new LokiHttpLoggerFactory(
                    options.HttpOptions,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.FormatterOptions);

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
    }
}
