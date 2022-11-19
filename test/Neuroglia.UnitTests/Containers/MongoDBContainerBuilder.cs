using DotNet.Testcontainers.Builders;
using System;
using System.IO;
using Xunit;

namespace Neuroglia.UnitTests.Containers
{

    public static class MongoDBContainerBuilder
    {

        static MongoDBContainer Container;

        public static MongoDBContainer Build()
        {
            if (Container != null)
                return Container;
            using var outputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
            Container = new TestcontainersBuilder<MongoDBContainer>()
                .WithName($"mongo-{Guid.NewGuid().ToString("N")}")
                .WithImage("mongo:latest")
                .WithPortBinding(MongoDBContainer.PublicPort, true)
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", MongoDBContainer.DefaultUsername)
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", MongoDBContainer.DefaultPassword)
                .WithOutputConsumer(outputConsumer)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilPortIsAvailable(MongoDBContainer.PublicPort)
                    .UntilMessageIsLogged(outputConsumer.Stdout, "Waiting for connections")
                )
                .Build();
            Container.StartAsync().GetAwaiter().GetResult();
            return Container;
        }

    }

}
