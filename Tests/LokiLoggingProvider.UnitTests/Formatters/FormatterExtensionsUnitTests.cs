namespace LokiLoggingProvider.UnitTests.Formatters
{
    using LokiLoggingProvider.Formatters;
    using LokiLoggingProvider.Options;
    using Xunit;

    public class FormatterExtensionsUnitTests
    {
        public class CreateFormatter
        {
            [Fact]
            public void When_CreatingFormatter_Expect_SimpleFormatter()
            {
                // Arrange
                Formatter formatter = Formatter.Simple;

                // Act
                ILogEntryFormatter result = formatter.CreateFormatter();

                // Assert
                Assert.IsType<SimpleFormatter>(result);
            }

            [Fact]
            public void When_CreatingFormatter_Expect_LogfmtFormatter()
            {
                // Arrange
                Formatter formatter = Formatter.Logfmt;

                // Act
                ILogEntryFormatter result = formatter.CreateFormatter();

                // Assert
                Assert.IsType<LogfmtFormatter>(result);
            }
        }
    }
}
