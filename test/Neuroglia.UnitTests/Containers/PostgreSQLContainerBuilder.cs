using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System;

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
            Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithName($"npgsql-{Guid.NewGuid():N}")
                .WithDatabase(new PostgreSqlTestcontainerConfiguration()
                {
                    Database = Database,
                    Username = Username,
                    Password = Password
                })
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilPortIsAvailable(5432))
                .Build();
            Container.StartAsync().GetAwaiter().GetResult();
            return Container;
        }

    }

}
