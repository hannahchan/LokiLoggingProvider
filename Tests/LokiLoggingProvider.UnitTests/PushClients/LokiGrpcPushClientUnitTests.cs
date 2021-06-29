namespace LokiLoggingProvider.UnitTests.PushClients
{
    using System;
    using Grpc.Net.Client;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.PushClients;
    using Xunit;

    public class LokiGrpcPushClientUnitTests
    {
        public class Dispose
        {
            [Fact]
            public void When_DisposingMoreThanOnce_Expect_NoExceptions()
            {
                // Arrange
                LokiGrpcPushClient pushClient = new LokiGrpcPushClient(GrpcChannel.ForAddress("http://localhost:9095"));

                // Act
                Exception result = Record.Exception(() =>
                {
                    pushClient.Dispose();
                    pushClient.Dispose();
                });

                // Assert
                Assert.Null(result);
            }
        }

        public class Push
        {
            [Fact]
            public void When_PushingLogMessageEntryWithDisposedPushClient_Expect_ObjectDisposedException()
            {
                // Arrange
                LokiGrpcPushClient pushClient = new LokiGrpcPushClient(GrpcChannel.ForAddress("http://localhost:9095"));
                pushClient.Dispose();

                LokiLogMessageEntry entry = new LokiLogMessageEntry(default, default, default);

                // Act
                Exception result = Record.Exception(() => pushClient.Push(entry));

                // Assert
                ObjectDisposedException objectDisposedException = Assert.IsType<ObjectDisposedException>(result);
                Assert.Equal(nameof(LokiGrpcPushClient), objectDisposedException.ObjectName);
            }
        }
    }
}
