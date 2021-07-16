namespace LokiLoggingProvider.UnitTests.Logger
{
    using System;
    using System.Collections.Generic;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Xunit;

    public class LabelValuesUnitTests
    {
        public class Constructor
        {
            [Fact]
            public void When_ConstructingLabelValuesWithStaticLabelOptions_Expect_LabelValues()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = nameof(StaticLabelOptions.JobName),
                    IncludeInstanceLabel = true,
                    AdditionalStaticLabels = new Dictionary<string, object>
                    {
                        { "job", "I should not be visible." },
                        { "instance", "I should not be visible." },
                        { "myString", "String" },
                        { "myNull", null },
                        { "my Integer", 123 },
                        { "my   Decimal", 123.123 },
                    },
                };

                // Act
                LabelValues result = new LabelValues(options);

                // Assert
                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal("instance", label.Key);
                        Assert.Equal(Environment.MachineName, label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("job", label.Key);
                        Assert.Equal(nameof(StaticLabelOptions.JobName), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("myDecimal", label.Key);
                        Assert.Equal("123.123", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("myInteger", label.Key);
                        Assert.Equal("123", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("myString", label.Key);
                        Assert.Equal("String", label.Value);
                    });
            }

            [Fact]
            public void When_ConstructingLabelValuesWithStaticLabelOptionsWithOnlyAdditionalStaticLabels_Expect_LabelValues()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = string.Empty,
                    IncludeInstanceLabel = false,
                    AdditionalStaticLabels = new Dictionary<string, object>
                    {
                        { "job", "I should be visible." },
                        { "instance", "I should be visible." },
                    },
                };

                // Act
                LabelValues result = new LabelValues(options);

                // Assert
                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal("instance", label.Key);
                        Assert.Equal("I should be visible.", label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("job", label.Key);
                        Assert.Equal("I should be visible.", label.Value);
                    });
            }

            [Fact]
            public void When_ConstructingLabelValuesWithStaticLabelOptions_Expect_EmptyLabelValues()
            {
                // Arrange
                StaticLabelOptions options = new StaticLabelOptions
                {
                    JobName = string.Empty,
                    IncludeInstanceLabel = false,
                    AdditionalStaticLabels = new Dictionary<string, object>(),
                };

                // Act
                LabelValues result = new LabelValues(options);

                // Assert
                Assert.Empty(result);
            }
        }

        public class AddDynamicLabels
        {
            [Fact]
            public void When_AddingDynamicLabels_Expect_DynamicLabels()
            {
                // Arrange
                LabelValues labelValues = new LabelValues();

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeCategory = true,
                    IncludeLogLevel = true,
                    IncludeEventId = true,
                    IncludeException = true,
                };

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Warning,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: new InvalidOperationException(),
                    formatter: (state, exception) => state.ToString());

                // Act
                LabelValues result = labelValues.AddDynamicLabels(options, logEntry);

                // Assert
                Assert.Empty(labelValues);
                Assert.NotEqual(labelValues, result);

                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal("category", label.Key);
                        Assert.Equal(logEntry.Category, label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("eventId", label.Key);
                        Assert.Equal(logEntry.EventId.ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("exception", label.Key);
                        Assert.Equal(logEntry.Exception.GetType().ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("logLevel", label.Key);
                        Assert.Equal(logEntry.LogLevel.ToString(), label.Value);
                    });
            }

            [Fact]
            public void When_AddingDynamicLabels_Expect_NoDynamicLabels()
            {
                // Arrange
                LabelValues labelValues = new LabelValues();

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeCategory = false,
                    IncludeLogLevel = false,
                    IncludeEventId = false,
                    IncludeException = false,
                };

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Warning,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: new InvalidOperationException(),
                    formatter: (state, exception) => state.ToString());

                // Act
                LabelValues result = labelValues.AddDynamicLabels(options, logEntry);

                // Assert
                Assert.Equal(labelValues, result);
                Assert.Empty(result);
            }

            [Fact]
            public void When_AddingDynamicLabels_Expect_DynamicLabelsToOverrideStaticLabels()
            {
                // Arrange
                LabelValues labelValues = new LabelValues(new StaticLabelOptions
                {
                    AdditionalStaticLabels = new Dictionary<string, object>
                    {
                        { "logLevel", "I should be overridden." },
                        { "category", "I should be overridden." },
                        { "myLabel", "I not should be overridden." },
                        { "eventId", "I should be overridden." },
                        { "exception", "I should be overridden." },
                    },
                });

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeCategory = true,
                    IncludeLogLevel = true,
                    IncludeEventId = true,
                    IncludeException = true,
                };

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Warning,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: new InvalidOperationException(),
                    formatter: (state, exception) => state.ToString());

                // Act
                LabelValues result = labelValues.AddDynamicLabels(options, logEntry);

                // Assert
                Assert.NotEqual(labelValues, result);

                Assert.Collection(
                    result,
                    label =>
                    {
                        Assert.Equal("category", label.Key);
                        Assert.Equal(logEntry.Category, label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("eventId", label.Key);
                        Assert.Equal(logEntry.EventId.ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("exception", label.Key);
                        Assert.Equal(logEntry.Exception.GetType().ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("instance", label.Key);
                        Assert.Equal(Environment.MachineName, label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("logLevel", label.Key);
                        Assert.Equal(logEntry.LogLevel.ToString(), label.Value);
                    },
                    label =>
                    {
                        Assert.Equal("myLabel", label.Key);
                        Assert.Equal("I not should be overridden.", label.Value);
                    });
            }

            [Fact]
            public void When_AddingDynamicLabelsIncludingExceptionWithNullException_Expect_NoDynamicLabels()
            {
                // Arrange
                LabelValues labelValues = new LabelValues();

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeException = true,
                };

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Warning,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                LabelValues result = labelValues.AddDynamicLabels(options, logEntry);

                // Assert
                Assert.Equal(labelValues, result);
                Assert.Empty(result);
            }

            [Fact]
            public void When_AddingDynamicLabelsNotIncludingExceptionWithNullException_Expect_NoDynamicLabels()
            {
                // Arrange
                LabelValues labelValues = new LabelValues();

                DynamicLabelOptions options = new DynamicLabelOptions
                {
                    IncludeException = false,
                };

                LogEntry<string> logEntry = new LogEntry<string>(
                    logLevel: LogLevel.Warning,
                    category: "MyCategory",
                    eventId: default,
                    state: "My Log Message.",
                    exception: null,
                    formatter: (state, exception) => state.ToString());

                // Act
                LabelValues result = labelValues.AddDynamicLabels(options, logEntry);

                // Assert
                Assert.Equal(labelValues, result);
                Assert.Empty(result);
            }
        }

        public class Set
        {
            [Fact]
            public void When_SettingLabelValues_Expect_ValuesSet()
            {
                // Arrange
                LabelValues labelValues = new LabelValues();

                // Act
                labelValues.SetJob(nameof(labelValues.SetJob));
                labelValues.SetInstance(nameof(labelValues.SetInstance));
                labelValues.SetCategory(nameof(labelValues.SetCategory));
                labelValues.SetLogLevel(nameof(labelValues.SetLogLevel));
                labelValues.SetEventId(nameof(labelValues.SetEventId));
                labelValues.SetException(nameof(labelValues.SetException));

                // Assert
                Assert.Collection(
                    labelValues,
                    keyValuePair =>
                    {
                        Assert.Equal("category", keyValuePair.Key);
                        Assert.Equal("SetCategory", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("eventId", keyValuePair.Key);
                        Assert.Equal("SetEventId", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("exception", keyValuePair.Key);
                        Assert.Equal("SetException", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("instance", keyValuePair.Key);
                        Assert.Equal("SetInstance", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("job", keyValuePair.Key);
                        Assert.Equal("SetJob", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("logLevel", keyValuePair.Key);
                        Assert.Equal("SetLogLevel", keyValuePair.Value);
                    });
            }
        }

        public class ToLabelString
        {
            [Fact]
            public void When_CallingToString_Expect_LabelString()
            {
                // Arrange
                LabelValues labelValues = new LabelValues
                {
                    { "MyString", "abc" },
                    { "MyInteger", "123" },
                    { "MyDecimal", "123.456" },
                    { "MyBool", "True" },
                };

                // Act
                string result = labelValues.ToString();

                // Assert
                Assert.Equal("MyBool=\"True\",MyDecimal=\"123.456\",MyInteger=\"123\",MyString=\"abc\"", result);
            }

            [Fact]
            public void When_CallingToStringWithEmptyLabelValues_Expect_EmptyString()
            {
                // Arrange
                LabelValues labelValues = new LabelValues();

                // Act
                string result = labelValues.ToString();

                // Assert
                Assert.Equal(string.Empty, result);
            }
        }
    }
}
