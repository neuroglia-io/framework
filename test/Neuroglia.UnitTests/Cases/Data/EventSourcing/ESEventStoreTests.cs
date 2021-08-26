using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.EventSourcing;
using Neuroglia.UnitTests.Containers;
using Neuroglia.UnitTests.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.EventSourcing
{

    [TestCaseOrderer("Xunit.PriorityTestCaseOrderer", "Neuroglia.Xunit")]
    public class ESEventStoreTests
        : IDisposable
    {

        public ESEventStoreTests()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddEventStore(builder => 
                builder.UseConnection(connection =>
                {
                    connection
                        .UseConnectionString(EventStoreContainerBuilder.Build().ConnectionString)
                        .ConfigureSettings(settings => settings.DisableTls());
                }));
            this.ServiceScope = services.BuildServiceProvider().CreateScope();
            this.EventStore = this.ServiceScope.ServiceProvider.GetRequiredService<ESEventStore>();
        }

        IServiceScope ServiceScope { get; }

        ESEventStore EventStore { get; }

        static readonly string StreamId = $"test-{Guid.NewGuid().ToString().Replace("-", "")}";

        [Fact, Priority(0)]
        public async Task AppendEventsToStream()
        {
            //arrange
            var events = new EventMetadata[]
            {
                new("FakeType", new TestPerson("Fake First Name", "Fake Last Name"))
            };

            //act
            await this.EventStore.AppendToStreamAsync(StreamId, events);
            var sourcedEvents = await this.EventStore.ReadAllEventsForwardAsync(StreamId);

            //assert
            sourcedEvents.Should().HaveCount(events.Length);
            sourcedEvents.First().Data.As<JObject>().ToObject<TestPerson>().Should().BeEquivalentTo(events.First().Data);
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
