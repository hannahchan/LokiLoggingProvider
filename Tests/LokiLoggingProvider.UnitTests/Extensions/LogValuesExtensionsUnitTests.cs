namespace LokiLoggingProvider.UnitTests.Extensions
{
    using System.Diagnostics;
    using LokiLoggingProvider.Extensions;
    using LokiLoggingProvider.Formatters;
    using Xunit;

    public class LogValuesExtensionsUnitTests
    {
        public class AddActivityTracking
        {
            [Fact]
            public void When_AddingActivityTracking_Expect_ActivityTrackingAdded()
            {
                // Arrange
                LogValues logValues = new LogValues();

                using Activity parentActivity = new Activity(nameof(parentActivity));
                using Activity childActivity = new Activity(nameof(parentActivity));

                // Act
                parentActivity.Start();
                childActivity.Start();

                logValues.AddActivityTracking();

                // Assert
                Assert.Collection(
                    logValues,
                    keyValuePair =>
                    {
                        Assert.Equal("SpanId", keyValuePair.Key);
                        Assert.Equal(childActivity.GetSpanId(), keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("TraceId", keyValuePair.Key);
                        Assert.Equal(childActivity.GetTraceId(), keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("ParentId", keyValuePair.Key);
                        Assert.Equal(parentActivity.GetSpanId(), keyValuePair.Value);
                    });
            }

            [Fact]
            public void When_AddingActivityTracking_Expect_ActivityTrackingNotAdded()
            {
                // Arrange
                LogValues logValues = new LogValues();

                // Act
                logValues.AddActivityTracking();

                // Assert
                Assert.Empty(logValues);
            }

            [Fact]
            public void When_AddingActivityTrackingToDictionaryWithExistingKeys_Expect_NoOverrides()
            {
                // Arrange
                LogValues logValues = new LogValues
                {
                    ["SpanId"] = "SpanId",
                    ["TraceId"] = "TraceId",
                    ["ParentId"] = "ParentId",
                };

                using Activity activity = new Activity(nameof(activity));

                // Act
                activity.Start();

                logValues.AddActivityTracking();

                // Assert
                Assert.Collection(
                    logValues,
                    keyValuePair =>
                    {
                        Assert.Equal("SpanId", keyValuePair.Key);
                        Assert.Equal("SpanId", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("TraceId", keyValuePair.Key);
                        Assert.Equal("TraceId", keyValuePair.Value);
                    },
                    keyValuePair =>
                    {
                        Assert.Equal("ParentId", keyValuePair.Key);
                        Assert.Equal("ParentId", keyValuePair.Value);
                    });
            }
        }
    }
}
