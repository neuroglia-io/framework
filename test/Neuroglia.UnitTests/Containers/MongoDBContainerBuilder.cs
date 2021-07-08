using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.WaitStrategies;

namespace Neuroglia.UnitTests.Containers
{

    public static class MongoDBContainerBuilder
    {

        static MongoDBContainer Container;

        public static MongoDBContainer Build()
        {
            if (Container != null)
                return Container;
            Container = new TestcontainersBuilder<MongoDBContainer>()
                .WithName("mongo")
                .WithImage("mongo:latest")
                .WithPortBinding(27017, true)
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", MongoDBContainer.DefaultUsername)
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", MongoDBContainer.DefaultPassword)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(27017)
                    .UntilCommandIsCompleted($"mongo {MongoDBContainer.DefaultDatabase}"))
                .Build();
            Container.StartAsync().GetAwaiter().GetResult();
            return Container;
        }

    }

}
