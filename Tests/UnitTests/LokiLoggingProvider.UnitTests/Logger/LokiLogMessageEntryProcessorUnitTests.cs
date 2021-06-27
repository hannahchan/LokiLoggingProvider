namespace LokiLoggingProvider.UnitTests.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LokiLoggingProvider.Logger;
    using LokiLoggingProvider.PushClients;
    using Xunit;

    public class LokiLogMessageEntryProcessorUnitTests
    {
        public class Dispose
        {
            [Fact]
            public void When_Disposed_Expext_MessageQueueIsCompleted()
            {
                // Arrange
                MockLokiPushClient client = new MockLokiPushClient();
                LokiLogMessageEntryProcessor processor = new LokiLogMessageEntryProcessor(client);

                // Act
                processor.Dispose();

                // Assert
                Assert.True(processor.MessageQueue.IsCompleted);
            }

            [Fact]
            public void When_DisposingMoreThanOnce_Expect_NoExceptions()
            {
                // Arrange
                MockLokiPushClient client = new MockLokiPushClient();
                LokiLogMessageEntryProcessor processor = new LokiLogMessageEntryProcessor(client);

                // Act
                Exception resul = Record.Exception(() =>
                {
                    processor.Dispose();
                    processor.Dispose();
                });

                // Assert
                Assert.Null(resul);
            }
        }

        public class EnqueueMessage
        {
            [Fact]
            public async Task When_EnqueuingMessage_Expect_MessageProcessed()
            {
                // Arrange
                MockLokiPushClient client = new MockLokiPushClient();
                LokiLogMessageEntryProcessor processor = new LokiLogMessageEntryProcessor(client);

                LokiLogMessageEntry message = new LokiLogMessageEntry(default, default, nameof(LokiLogMessageEntry.Message));

                // Act
                processor.EnqueueMessage(message);

                do
                {
                    // Allow some time for background thread to process queue
                    await Task.Delay(100);
                }
                while (processor.MessageQueue.Any());

                // Assert
                Assert.Contains(message, client.GetReceivedLogMessageEntries());
            }

            [Fact]
            public async Task When_EnqueuingMessageAndMessageQueueIsCompleted_Expect_NoMessageProcessed()
            {
                // Arrange
                MockLokiPushClient client = new MockLokiPushClient();
                LokiLogMessageEntryProcessor processor = new LokiLogMessageEntryProcessor(client);
                processor.MessageQueue.CompleteAdding();

                LokiLogMessageEntry message = new LokiLogMessageEntry(default, default, nameof(LokiLogMessageEntry.Message));

                // Act
                processor.EnqueueMessage(message);

                do
                {
                    // Allow some time for background thread to process queue
                    await Task.Delay(100);
                }
                while (processor.MessageQueue.Any());

                // Assert
                Assert.Empty(client.GetReceivedLogMessageEntries());
            }

            [Fact]
            public void When_EnqueuingMessageWithDisposedLogMessageEntryProcessor_Expect_ObjectDisposedException()
            {
                // Arrange
                MockLokiPushClient client = new MockLokiPushClient();
                LokiLogMessageEntryProcessor processor = new LokiLogMessageEntryProcessor(client);
                processor.Dispose();

                LokiLogMessageEntry message = new LokiLogMessageEntry(default, default, nameof(LokiLogMessageEntry.Message));

                // Act
                Exception result = Record.Exception(() => processor.EnqueueMessage(message));

                // Assert
                ObjectDisposedException objectDisposedException = Assert.IsType<ObjectDisposedException>(result);
                Assert.Equal(nameof(LokiLogMessageEntryProcessor), objectDisposedException.ObjectName);
            }
        }

        private sealed class MockLokiPushClient : ILokiPushClient
        {
            private readonly IList<LokiLogMessageEntry> receivedLogMessageEntries = new List<LokiLogMessageEntry>();

            public void Dispose()
            {
                this.receivedLogMessageEntries.Clear();
            }

            public void Push(LokiLogMessageEntry entry)
            {
                this.receivedLogMessageEntries.Add(entry);
            }

            public IEnumerable<LokiLogMessageEntry> GetReceivedLogMessageEntries()
            {
                return this.receivedLogMessageEntries;
            }
        }
    }
}
