namespace LokiLoggingProvider.Logger
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using LokiLoggingProvider.PushClients;

    internal sealed class LokiLogMessageEntryProcessor : IDisposable
    {
        private const int MaxQueuedMessages = 1024;

        private readonly ILokiPushClient client;

        private readonly Thread backgroundThread;

        private bool disposed;

        public LokiLogMessageEntryProcessor(ILokiPushClient client)
        {
            this.client = client;

            this.backgroundThread = new Thread(this.ProcessLogQueue)
            {
                IsBackground = true,
                Name = nameof(LokiLogMessageEntryProcessor),
            };

            this.backgroundThread.Start();
        }

        // Internal for testing
        internal BlockingCollection<LokiLogMessageEntry> MessageQueue { get; } = new BlockingCollection<LokiLogMessageEntry>(MaxQueuedMessages);

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.MessageQueue.CompleteAdding();

            try
            {
                this.backgroundThread.Join();
            }
            catch (ThreadStateException)
            {
                // Do nothing
            }

            this.client.Dispose();
            this.disposed = true;
        }

        public void EnqueueMessage(LokiLogMessageEntry message)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(LokiLogMessageEntryProcessor));
            }

            if (this.MessageQueue.IsAddingCompleted)
            {
                return;
            }

            try
            {
                this.MessageQueue.Add(message);
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (LokiLogMessageEntry entry in this.MessageQueue.GetConsumingEnumerable())
                {
                    this.PushMessage(entry);
                }
            }
            catch
            {
                try
                {
                    this.MessageQueue.CompleteAdding();
                }
                catch
                {
                    // Do nothing
                }
            }
        }

        private void PushMessage(LokiLogMessageEntry entry)
        {
            try
            {
                this.client.Push(entry);
            }
            catch
            {
                // Do nothing
            }
        }
    }
}
