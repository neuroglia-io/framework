using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System;
using System.IO;

namespace Neuroglia.UnitTests.Containers
{

    public static class PostgreSQLContainerBuilder
    {

        public const string Database = "test";
        public const string Username = "test";
        public const string Password = "test";

        static PostgreSqlTestcontainer Container;

        public static PostgreSqlTestcontainer Build()
        {
            if (Container != null)
                return Container;
            using var outputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
            Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithName($"npgsql-{Guid.NewGuid():N}")
                .WithDatabase(new PostgreSqlTestcontainerConfiguration()
                {
                    Database = Database,
                    Username = Username,
                    Password = Password
                })
                .WithOutputConsumer(outputConsumer)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(5432)
                    .UntilMessageIsLogged(outputConsumer.Stdout, "database system is ready to accept connections")
                )
                .Build();
            Container.StartAsync().GetAwaiter().GetResult();
            return Container;
        }

    }

}
