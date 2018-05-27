using System;
using Hangfire.Annotations;
using Hangfire.Logging;
using Hangfire.Storage;

namespace Hangfire.Postgres.RabbitMq
{
    internal class RabbitMqFetchedJob : IFetchedJob
    {
        private readonly Action _removeFromQueue;
        private readonly Action _requeue;
        private bool _completed;
        private bool _disposed;

        private static readonly ILog Logger = LogProvider.For<RabbitMqJobQueue>();

        public RabbitMqFetchedJob(string jobId, [NotNull] Action removeFromQueue, [NotNull] Action requeue)
        {
            _removeFromQueue = removeFromQueue ?? throw new ArgumentNullException(nameof(removeFromQueue));
            _requeue = requeue ?? throw new ArgumentNullException(nameof(requeue));

            JobId = jobId;
            Logger.Debug($"Job dequeued: {JobId}");
        }

        public string JobId { get; private set; }

        public void RemoveFromQueue()
        {
            if (_completed) throw new InvalidOperationException("Job already completed");
            _removeFromQueue();
            _completed = true;
            Logger.Debug($"Job ACK'ed: {JobId}");
        }

        public void Requeue()
        {
            if (_completed) throw new InvalidOperationException("Job already completed");
            _requeue();

            _completed = true;
        }

        public void Dispose()
        {
            if (!_completed && !_disposed)
            {
                Requeue();
            }

            _disposed = true;
        }
    }
}
