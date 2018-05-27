using System;
using System.Data;
using Hangfire.Annotations;
using Hangfire.PostgreSql;
using RabbitMQ.Client;

namespace Hangfire.Postgres.RabbitMq
{
    internal class RabbitMqJobQueueProvider : IPersistentJobQueueProvider, IDisposable
    {
        private readonly RabbitMqJobQueue _jobQueue;
        private readonly RabbitMqMonitoringApi _monitoringApi;

        public RabbitMqJobQueueProvider(string[] queues, ConnectionFactory configureAction,
            [CanBeNull] Action<IModel> configureConsumer = null)
        {
            if (queues == null) throw new ArgumentNullException(nameof(queues));
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            _jobQueue = new RabbitMqJobQueue(queues, configureAction, configureConsumer);
            _monitoringApi = new RabbitMqMonitoringApi(configureAction, queues);
        }

        public IPersistentJobQueue GetJobQueue()
        {
            return _jobQueue;
        }

        public void Dispose()
        {
            _jobQueue.Dispose();
        }

        public IPersistentJobQueue GetJobQueue(IDbConnection connection)
        {
            return _jobQueue;
        }

        public IPersistentJobQueueMonitoringApi GetJobQueueMonitoringApi(IDbConnection connection)
        {
            return _monitoringApi;
        }
    }
}
