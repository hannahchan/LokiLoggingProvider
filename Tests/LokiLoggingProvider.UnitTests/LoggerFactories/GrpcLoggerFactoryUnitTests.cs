namespace LokiLoggingProvider.UnitTests.LoggerFactories
{
    using System;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.LoggerFactories;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class GrpcLoggerFactoryUnitTests
    {
        public class CreateLogger
        {
            [Fact]
            public void When_CreatingLogger_Expect_LoggerCreated()
            {
                // Arrange
                LokiLoggerOptions options = new LokiLoggerOptions();
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.GrpcOptions,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.Formatter);

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
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.GrpcOptions,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.Formatter);

                loggerFactory.Dispose();

                string categoryName = nameof(categoryName);

                // Act
                Exception result = Record.Exception(() => loggerFactory.CreateLogger(categoryName));

                // Assert
                ObjectDisposedException objectDisposedException = Assert.IsType<ObjectDisposedException>(result);
                Assert.Equal(nameof(GrpcLoggerFactory), objectDisposedException.ObjectName);
            }
        }

        public class Dispose
        {
            [Fact]
            public void When_DisposingMoreThanOnce_Expect_NoExceptions()
            {
                // Arrange
                LokiLoggerOptions options = new LokiLoggerOptions();
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.GrpcOptions,
                    options.StaticLabelOptions,
                    options.DynamicLabelOptions,
                    options.Formatter);

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
