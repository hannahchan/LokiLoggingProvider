namespace LokiLoggingProvider.Logger
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using LokiLoggingProvider.PushClients;

    internal sealed class LokiLogEntryProcessor : IDisposable
    {
        private const int MaxQueuedMessages = 1024;

        private readonly ILokiPushClient client;

        private readonly Thread backgroundThread;

        private bool disposed;

        public LokiLogEntryProcessor(ILokiPushClient client)
        {
            this.client = client;

            this.backgroundThread = new Thread(this.ProcessLogQueue)
            {
                IsBackground = true,
                Name = nameof(LokiLogEntryProcessor),
            };

            this.backgroundThread.Start();
        }

        // Internal for testing
        internal BlockingCollection<LokiLogEntry> MessageQueue { get; } = new BlockingCollection<LokiLogEntry>(MaxQueuedMessages);

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

        public void EnqueueMessage(LokiLogEntry message)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(LokiLogEntryProcessor));
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
                foreach (LokiLogEntry entry in this.MessageQueue.GetConsumingEnumerable())
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

        private void PushMessage(LokiLogEntry entry)
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
