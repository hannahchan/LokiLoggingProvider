namespace LokiLoggingProvider.UnitTests.Labels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using LokiLoggingProvider.Labels;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class LabelExtensionsUnitTests
    {
        public class ValidateAndThrow
        {
            [Fact]
            public void When_ValidatingStaticLabelOptions_Expect_NoExceptions()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions();

                // Act
                Exception result = Record.Exception(() => options.ValidateAndThrow());

                // Assert
                Assert.Null(result);
            }

            [Theory]
            [InlineData(Labels.Job)]
            [InlineData(Labels.Instance)]
            [InlineData(Labels.CategoryName)]
            [InlineData(Labels.LogLevel)]
            [InlineData(Labels.EventId)]
            [InlineData(Labels.Exception)]
            public void When_ValidatingStaticLabelOptions_Expect_InvalidOperationException(string label)
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    AdditionalStaticLabels = new Dictionary<string, object>
                    {
                        { label, label },
                    },
                };

                // Act
                Exception result = Record.Exception(() => options.ValidateAndThrow());

                // Assert
                InvalidOperationException invalidOperationException = Assert.IsType<InvalidOperationException>(result);
                Assert.Equal($"Reserved labels cannot be added to '{nameof(options.AdditionalStaticLabels)}'.", invalidOperationException.Message);
            }
        }

        public class ToReadOnlyDictionary
        {
            [Fact]
            public void When_JobNameIsSet_Expect_JobKeyInDictionary()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = nameof(options.JobName),
                };

                // Act
                IReadOnlyDictionary<string, string> result = options.ToReadOnlyDictionary();

                // Assert
                Assert.Contains(new KeyValuePair<string, string>(Labels.Job, nameof(options.JobName)), result);
            }

            [Fact]
            public void When_JobNameIsEmpty_Expect_NoJobKeyInDictionary()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = string.Empty,
                };

                // Act
                IReadOnlyDictionary<string, string> result = options.ToReadOnlyDictionary();

                // Assert
                Assert.False(result.TryGetValue(Labels.Job, out _));
            }

            [Fact]
            public void When_IncludeInstanceLabelIsTrue_Expect_InstanceKeyInDictionary()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    IncludeInstanceLabel = true,
                };

                // Act
                IReadOnlyDictionary<string, string> result = options.ToReadOnlyDictionary();

                // Assert
                Assert.Contains(new KeyValuePair<string, string>(Labels.Instance, Environment.MachineName), result);
            }

            [Fact]
            public void When_IncludeInstanceLabelIsFalse_Expect_NoInstanceKeyInDictionary()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    IncludeInstanceLabel = false,
                };

                // Act
                IReadOnlyDictionary<string, string> result = options.ToReadOnlyDictionary();

                // Assert
                Assert.False(result.TryGetValue(Labels.Instance, out _));
            }

            [Fact]
            public void When_AdditionalStaticLablesIsSet_Expect_AdditionalStaticKeysAndValuesInDictioinary()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = nameof(options.JobName),
                    IncludeInstanceLabel = true,
                    AdditionalStaticLabels = new Dictionary<string, object>
                    {
                        { "MyString", "abc" },
                        { "MyInteger", 123 },
                        { "MyDecimal", 123.456 },
                        { "MyBool", true },
                        { "MyNull", null },
                    },
                };

                // Act
                IReadOnlyDictionary<string, string> result = options.ToReadOnlyDictionary();

                // Assert
                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal(Labels.Job, label.Key);
                        Assert.Equal(nameof(options.JobName), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal(Labels.Instance, label.Key);
                        Assert.Equal(Environment.MachineName, label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("MyString", label.Key);
                        Assert.Equal("abc", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("MyInteger", label.Key);
                        Assert.Equal("123", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("MyDecimal", label.Key);
                        Assert.Equal("123.456", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("MyBool", label.Key);
                        Assert.Equal("True", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("MyNull", label.Key);
                        Assert.Equal(string.Empty, label.Value);
                    });
            }

            [Fact]
            public void When_AdditionalStaticLablesIsEmpty_Expect_NoAdditionalStaticKeysAndValuesInDictioinary()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = nameof(options.JobName),
                    IncludeInstanceLabel = true,
                    AdditionalStaticLabels = new Dictionary<string, object>(),
                };

                // Act
                IReadOnlyDictionary<string, string> result = options.ToReadOnlyDictionary();

                // Assert
                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal(Labels.Job, label.Key);
                        Assert.Equal(nameof(options.JobName), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal(Labels.Instance, label.Key);
                        Assert.Equal(Environment.MachineName, label.Value);
                    });
            }
        }

        public class AddDynamicLables
        {
            private readonly string categoryName = nameof(categoryName);

            private readonly LogLevel logLevel = default;

            private readonly EventId eventId = default;

            private readonly InvalidOperationException exception = new InvalidOperationException();

            [Fact]
            public void When_AllDynamicLabelsAreIncluded_Expect_AllDynamicLabelsIncluded()
            {
                // Arrange
                IReadOnlyDictionary<string, string> labels = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeCategoryName = true,
                    IncludeLogLevel = true,
                    IncludeEventId = true,
                    IncludeException = true,
                };

                // Act
                IReadOnlyDictionary<string, string> result = labels.AddDynamicLables(options, this.categoryName, this.logLevel, this.eventId, this.exception);

                // Assert
                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal(Labels.CategoryName, label.Key);
                        Assert.Equal(this.categoryName, label.Value);
                    },
                    label =>
                    {
                        Assert.Equal(Labels.LogLevel, label.Key);
                        Assert.Equal(this.logLevel.ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal(Labels.EventId, label.Key);
                        Assert.Equal(this.eventId.ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal(Labels.Exception, label.Key);
                        Assert.Equal(typeof(InvalidOperationException).ToString(), label.Value);
                    });
            }

            [Fact]
            public void When_AllDynamicLabelsAreNotIncluded_Expect_NoDynamicLabelsIncluded()
            {
                // Arrange
                IReadOnlyDictionary<string, string> labels = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeCategoryName = false,
                    IncludeLogLevel = false,
                    IncludeEventId = false,
                    IncludeException = false,
                };

                // Act
                IReadOnlyDictionary<string, string> result = labels.AddDynamicLables(options, this.categoryName, this.logLevel, this.eventId, this.exception);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void When_ExceptionLabelIsIncludedWithNullException_Expect_NoExceptionLabelIncluded()
            {
                // Arrange
                IReadOnlyDictionary<string, string> labels = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeException = true,
                };

                // Act
                IReadOnlyDictionary<string, string> result = labels.AddDynamicLables(options, this.categoryName, this.logLevel, this.eventId, null);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void When_ExceptionLabelIsNotIncludedWithNullException_Expect_NoExceptionLabelIncluded()
            {
                // Arrange
                IReadOnlyDictionary<string, string> labels = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeException = false,
                };

                // Act
                IReadOnlyDictionary<string, string> result = labels.AddDynamicLables(options, this.categoryName, this.logLevel, this.eventId, null);

                // Assert
                Assert.Empty(result);
            }
        }

        public class ToLabelString
        {
            [Fact]
            public void When_GettingLabelStringFromDictionary_Expect_LabelString()
            {
                // Arrange
                Dictionary<string, string> labels = new Dictionary<string, string>
                {
                    { "MyString", "abc" },
                    { "MyInteger", "123" },
                    { "MyDecimal", "123.456" },
                    { "MyBool", "True" },
                };

                // Act
                string result = labels.ToLabelString();

                // Assert
                Assert.Equal("{MyString=\"abc\",MyInteger=\"123\",MyDecimal=\"123.456\",MyBool=\"True\"}", result);
            }

            [Fact]
            public void When_GettingLabelStringFromEmptyDictionary_Expect_EmptyLabelString()
            {
                // Arrange
                Dictionary<string, string> labels = new Dictionary<string, string>();

                // Act
                string result = labels.ToLabelString();

                // Assert
                Assert.Equal("{}", result);
            }
        }
    }
}
