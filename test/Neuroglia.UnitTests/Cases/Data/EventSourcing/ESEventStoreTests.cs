using EventStore.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.EventSourcing;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Containers;
using Neuroglia.UnitTests.Data.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = new();
                settings.ContractResolver = new NonPublicSetterContractResolver();
                return settings;
            };
            ServiceCollection services = new();
            services.AddLogging();
            services.AddNewtonsoftJsonSerializer(options =>
            {
                options.ContractResolver = new NonPublicSetterContractResolver();
            });
            services.AddEventStore(builder => 
                builder.ConfigureClient(client =>
                {
                    client.UseConnectionString(EventStoreContainerBuilder.Build().ConnectionString);
                }));
            this.ServiceScope = services.BuildServiceProvider().CreateScope();
            this.EventStore = this.ServiceScope.ServiceProvider.GetRequiredService<ESEventStore>();
        }

        IServiceScope ServiceScope { get; }

        ESEventStore EventStore { get; }

        static readonly string StreamId = $"test-{Guid.NewGuid().ToString().Replace("-", "")}";

        static int ConsumedEvents;

        static string SubscriptionId;

        [Fact, Priority(0)]
        public async Task AppendEventsToStream()
        {
            //arrange
            var payload = new TestPersonCreatedDomainEvent(Guid.NewGuid(), "Fake First Name", "Fake Last Name");
            var events = new EventMetadata[]
            {
                new("FakeType", payload)
            };

            //act
            await this.EventStore.AppendToStreamAsync(StreamId, events);
            var sourcedEvents = await this.EventStore.ReadAllEventsForwardAsync(StreamId);

            //assert
            sourcedEvents.Should().HaveCount(events.Length);
            sourcedEvents.First().Data.As<JObject>().ToObject<TestPersonCreatedDomainEvent>().Should().BeEquivalentTo(events.First().Data);

            events = new EventMetadata[]
            {
                new("FakeType", payload)
            };
            Func<Task> act = () => this.EventStore.AppendToStreamAsync(StreamId, events);
            await act.Should().ThrowAsync<WrongExpectedVersionException>();
        }

        [Fact, Priority(1)]
        public async Task GetStream()
        {
            //act
            var stream = await this.EventStore.GetStreamAsync(StreamId);

            //assert
            stream.Should().NotBeNull();
            stream.Id.Should().Be(StreamId);
            stream.Length.Should().Be(1);
        }

        [Fact, Priority(2)]
        public async Task SubscribeToStreamThenUnsubscribe()
        {
            //act
            SubscriptionId = await this.EventStore.SubscribeToStreamAsync(StreamId, OnEventAsync);
            await Task.Delay(50);

            //assert
            ConsumedEvents.Should().Be(1);

            //act
            this.EventStore.UnsubscribeFrom(SubscriptionId);
            await this.EventStore.AppendToStreamAsync(StreamId, new EventMetadata[]
            {
                new("FakeType", new TestPersonCreatedDomainEvent(Guid.NewGuid(), "Fake First Name", "Fake Last Name"))
            }, 0);
            await Task.Delay(50);

            //assert
            ConsumedEvents.Should().Be(1);
        }

        [Fact, Priority(3)]
        public async Task DeleteStream()
        {
            //act
            await this.EventStore.DeleteStreamAsync(StreamId);

            //assert
            (await this.EventStore.GetStreamAsync(StreamId)).Should().BeNull();
        }

        static Task OnEventAsync(IServiceProvider services, ISourcedEvent e)
        {
            ConsumedEvents++;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.ServiceScope.Dispose();
        }

    }

}
