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
            LogValues logValues = new();

            // Act
            logValues.SetLogLevel(nameof(logValues.SetLogLevel));
            logValues.SetCategory(nameof(logValues.SetCategory));
            logValues.SetEventId(nameof(logValues.SetEventId));
            logValues.SetMessage(nameof(logValues.SetMessage));
            logValues.SetState(nameof(logValues.SetState));
            logValues.SetScopes(nameof(logValues.SetScopes));
            logValues.SetException(nameof(logValues.SetException));
            logValues.SetExceptionDetails(nameof(logValues.SetExceptionDetails));

            // Assert
            Assert.Collection(
                logValues,
                keyValuePair =>
                {
                    Assert.Equal("Level", keyValuePair.Key);
                    Assert.Equal("SetLogLevel", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Category", keyValuePair.Key);
                    Assert.Equal("SetCategory", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("EventId", keyValuePair.Key);
                    Assert.Equal("SetEventId", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Message", keyValuePair.Key);
                    Assert.Equal("SetMessage", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("State", keyValuePair.Key);
                    Assert.Equal("SetState", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Scopes", keyValuePair.Key);
                    Assert.Equal("SetScopes", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("Exception", keyValuePair.Key);
                    Assert.Equal("SetException", keyValuePair.Value);
                },
                keyValuePair =>
                {
                    Assert.Equal("ExceptionDetails", keyValuePair.Key);
                    Assert.Equal("SetExceptionDetails", keyValuePair.Value);
                });
        }
    }
}
