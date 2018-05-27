using System;
using Hangfire.PostgreSql;
using RabbitMQ.Client;

namespace Hangfire.Postgres.RabbitMq
{
    public static class RabbitMqPostgreSqlStorageExtensions
    {
        public static PostgreSqlStorage UseRabbitMq(this PostgreSqlStorage storage, Action<RabbitMqConnectionConfiguration> configureAction, params string[] queues)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));
            if (queues == null) throw new ArgumentNullException(nameof(queues));
            if (queues.Length == 0) throw new ArgumentException("No queue(s) specified for RabbitMQ provider.", nameof(queues));
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));

            var conf = new RabbitMqConnectionConfiguration();
            configureAction(conf);

            var cf = new ConnectionFactory();

            // Use configuration from URI, otherwise use properties
            if (conf.Uri != null)
            {
                cf.Uri = conf.Uri;
            }
            else
            {
                cf.HostName = conf.HostName;
                cf.Port = conf.Port;
                cf.UserName = conf.Username;
                cf.Password = conf.Password;
                cf.VirtualHost = conf.VirtualHost;
            }

            var provider = new RabbitMqJobQueueProvider(queues, cf, channel =>
                channel.BasicQos(0, conf.PrefetchCount,
                    false // applied separately to each new consumer on the channel
                ));

            storage.QueueProviders.Add(provider, queues);

            return storage;
        }
    }
}
