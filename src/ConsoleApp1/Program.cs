using System;
using ClassLibrary1;
using Hangfire;
using Hangfire.Postgres.RabbitMq;
using Hangfire.PostgreSql;
using Hangfire.SimpleInjector;
using SimpleInjector;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
	        //var memoryStorage = new MemoryStorage();

            var postgreSqlStorage = new PostgreSqlStorage("server=127.0.0.1;database=hangfire_sample;uid=postgres;password=password")
                    .UseRabbitMq(conf =>
                {
                    conf.HostName = "localhost";
                }, "default");

            var container = new Container();

            container.Register(typeof(IX<>),new []{typeof(IX<>).Assembly});

            var server = new BackgroundJobServer(new BackgroundJobServerOptions()
            {
                Activator = new SimpleInjectorJobActivator(container)
            }, postgreSqlStorage);

            Console.WriteLine("server started");
			Console.ReadKey();

            server.Dispose();

        }
    }
}
