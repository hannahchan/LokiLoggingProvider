namespace LokiLoggingProvider.UnitTests.Formatters
{
    using LokiLoggingProvider.Formatters;
    using Xunit;

    public class LogValuesUnitTests
    {
        [Fact]
        public void When_SettingLogValues_Expect_ValuesSet()
        {
            // Arrange
            LogValues logValues = new LogValues();

            // Act
            logValues.LogLevel = nameof(logValues.LogLevel);
            logValues.Category = nameof(logValues.Category);
            logValues.EventId = nameof(logValues.EventId);
            logValues.Message = nameof(logValues.Message);
            logValues.State = nameof(logValues.State);
            logValues.Exception = nameof(logValues.Exception);
            logValues.ExceptionDetails = nameof(logValues.ExceptionDetails);

            // Assert
            Assert.Collection(
                logValues,
                keyValuePair =>
                {
                    Assert.Equal("LogLevel", keyValuePair.Key);
                    Assert.Equal("LogLevel", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Category", keyValuePair.Key);
                    Assert.Equal("Category", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("EventId", keyValuePair.Key);
                    Assert.Equal("EventId", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Message", keyValuePair.Key);
                    Assert.Equal("Message", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("State", keyValuePair.Key);
                    Assert.Equal("State", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Exception", keyValuePair.Key);
                    Assert.Equal("Exception", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("ExceptionDetails", keyValuePair.Key);
                    Assert.Equal("ExceptionDetails", keyValuePair.Value);
                });
        }

        [Fact]
        public void When_GettingLogValues_Expect_LogValues()
        {
            // Arrange
            LogValues logValues = new LogValues
            {
                LogLevel = nameof(logValues.LogLevel),
                Category = nameof(logValues.Category),
                EventId = nameof(logValues.EventId),
                Message = nameof(logValues.Message),
                State = nameof(logValues.State),
                Exception = nameof(logValues.Exception),
                ExceptionDetails = nameof(logValues.ExceptionDetails),
            };

            // Act and Assert
            Assert.Equal("LogLevel", logValues.LogLevel);
            Assert.Equal("Category", logValues.Category);
            Assert.Equal("EventId", logValues.EventId);
            Assert.Equal("Message", logValues.Message);
            Assert.Equal("State", logValues.State);
            Assert.Equal("Exception", logValues.Exception);
            Assert.Equal("ExceptionDetails", logValues.ExceptionDetails);
        }
    }
}
