namespace LokiLoggingProvider.UnitTests.LoggerFactories
{
    using System;
    using LokiLoggingProvider.Formatters;
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
                LokiLoggerOptions options = new();
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.Grpc,
                    options.StaticLabels,
                    options.DynamicLabels,
                    new SimpleFormatter(new SimpleFormatterOptions()));

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
                LokiLoggerOptions options = new();
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.Grpc,
                    options.StaticLabels,
                    options.DynamicLabels,
                    new SimpleFormatter(new SimpleFormatterOptions()));

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
                LokiLoggerOptions options = new();
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.Grpc,
                    options.StaticLabels,
                    options.DynamicLabels,
                    new SimpleFormatter(new SimpleFormatterOptions()));

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
            public void When_SettingScopeProvider_Expect_ScopeProviderSet()
            {
                // Arrange
                LokiLoggerOptions options = new();
                ILokiLoggerFactory loggerFactory = new GrpcLoggerFactory(
                    options.Grpc,
                    options.StaticLabels,
                    options.DynamicLabels,
                    new SimpleFormatter(new SimpleFormatterOptions()));

                string categoryName1 = nameof(categoryName1);
                string categoryName2 = nameof(categoryName2);

                ILogger logger1 = loggerFactory.CreateLogger(categoryName1);
                ILogger logger2 = loggerFactory.CreateLogger(categoryName2);

                // Act
                loggerFactory.SetScopeProvider(NullExternalScopeProvider.Instance);

                // Arrange
                LokiLogger lokiLogger1 = Assert.IsType<LokiLogger>(logger1);
                Assert.IsType<NullExternalScopeProvider>(lokiLogger1.ScopeProvider);

                LokiLogger lokiLogger2 = Assert.IsType<LokiLogger>(logger2);
                Assert.IsType<NullExternalScopeProvider>(lokiLogger2.ScopeProvider);
            }
        }
    }
}
