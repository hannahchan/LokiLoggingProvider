namespace LokiLoggingProvider.UnitTests.Extensions;

using System;
using LokiLoggingProvider.Extensions;
using LokiLoggingProvider.Formatters;
using LokiLoggingProvider.LoggerFactories;
using LokiLoggingProvider.Options;
using Xunit;

public class LokiLoggerOptionsExtensionsUnitTests
{
    public class CreateFormatter
    {
        [Theory]
        [InlineData(Formatter.Json, typeof(JsonFormatter))]
        [InlineData(Formatter.Logfmt, typeof(LogfmtFormatter))]
        [InlineData(Formatter.Simple, typeof(SimpleFormatter))]
        [InlineData((Formatter)100, typeof(SimpleFormatter))] // Invalid Formatter
        public void When_CreatingFormatter_Expect_Formatter(Formatter formatter, Type expectedType)
        {
            // Arrange
            LokiLoggerOptions options = new() { Formatter = formatter };

            // Act
            ILogEntryFormatter result = options.CreateFormatter();

            // Assert
            Assert.Equal(expectedType, result.GetType());
        }
    }

    public class CreateLoggerFactory
    {
        [Theory]
        [InlineData(PushClient.None, typeof(NullLoggerFactory))]
        [InlineData(PushClient.Grpc, typeof(GrpcLoggerFactory))]
        [InlineData(PushClient.Http, typeof(HttpLoggerFactory))]
        [InlineData((PushClient)100, typeof(NullLoggerFactory))] // Invalid Push Client
        public void When_CreatingLoggerFactory_Expect_LoggerFactory(PushClient client, Type expectedType)
        {
            // Arrange
            LokiLoggerOptions options = new() { Client = client };

            // Act
            ILokiLoggerFactory result = options.CreateLoggerFactory();

            // Assert
            Assert.Equal(expectedType, result.GetType());
        }
    }
}
