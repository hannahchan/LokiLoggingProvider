namespace LokiLoggingProvider.UnitTests.Logger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LokiLoggingProvider.Logger;
using LokiLoggingProvider.PushClients;
using Xunit;

public class LokiLogEntryProcessorUnitTests
{
    public class Dispose
    {
        [Fact]
        public void When_Disposed_Expext_MessageQueueIsCompleted()
        {
            // Arrange
            MockLokiPushClient client = new();
            LokiLogEntryProcessor processor = new(client);

            // Act
            processor.Dispose();

            // Assert
            Assert.True(processor.MessageQueue.IsCompleted);
        }

        [Fact]
        public void When_DisposingMoreThanOnce_Expect_NoExceptions()
        {
            // Arrange
            MockLokiPushClient client = new();
            LokiLogEntryProcessor processor = new(client);

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
            MockLokiPushClient client = new();
            LokiLogEntryProcessor processor = new(client);

            LokiLogEntry message = new(default, default, nameof(LokiLogEntry.Message));

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
            MockLokiPushClient client = new();
            LokiLogEntryProcessor processor = new(client);
            processor.MessageQueue.CompleteAdding();

            LokiLogEntry message = new(default, default, nameof(LokiLogEntry.Message));

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
            MockLokiPushClient client = new();
            LokiLogEntryProcessor processor = new(client);
            processor.Dispose();

            LokiLogEntry message = new(default, default, nameof(LokiLogEntry.Message));

            // Act
            Exception result = Record.Exception(() => processor.EnqueueMessage(message));

            // Assert
            ObjectDisposedException objectDisposedException = Assert.IsType<ObjectDisposedException>(result);
            Assert.Equal(nameof(LokiLogEntryProcessor), objectDisposedException.ObjectName);
        }
    }

    private sealed class MockLokiPushClient : ILokiPushClient
    {
        private readonly IList<LokiLogEntry> receivedLogMessageEntries = new List<LokiLogEntry>();

        public void Dispose()
        {
            this.receivedLogMessageEntries.Clear();
        }

        public void Push(LokiLogEntry entry)
        {
            this.receivedLogMessageEntries.Add(entry);
        }

        public IEnumerable<LokiLogEntry> GetReceivedLogMessageEntries()
        {
            return this.receivedLogMessageEntries;
        }
    }
}
