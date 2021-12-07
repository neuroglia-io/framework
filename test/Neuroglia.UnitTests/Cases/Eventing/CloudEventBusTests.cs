using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Neuroglia.Data;
using Neuroglia.Eventing.Services;
using Neuroglia.UnitTests.Factories;
using Neuroglia.UnitTests.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Eventing
{

    public class CloudEventBusTests
    {

        public CloudEventBusTests()
        {
            
        }

        [Fact, Priority(0)]
        public async Task Publish_NoOutbox_ShouldWork()
        {
            //arrange
            var cloudEventBus = CloudEventBusFactory.Create();
            await cloudEventBus.StartAsync(default);
            var e = CloudEventFactory.Create();
            TestHttpMessageHandler.SendTaskSource = new();

            //act
            await cloudEventBus.PublishAsync(e);
            await TestHttpMessageHandler.SendTaskSource.Task;
        }

        [Fact, Priority(1)]
        public async Task Publish_WithOutbox_ShouldWork()
        {
            //arrange
            CloudEventOutboxEntry outboxEntry = null;
            var services = new ServiceCollection();
            services.AddSingleton(provider =>
            {
                var repo = new Mock<IRepository<CloudEventOutboxEntry, string>>();
                repo.Setup(r => r.AddAsync(It.IsAny<CloudEventOutboxEntry>(), It.IsAny<CancellationToken>()))
                    .Returns((CloudEventOutboxEntry entry, CancellationToken ct) =>
                    {
                        outboxEntry = entry;
                        return Task.FromResult(entry);
                    });
                repo.Setup(r => r.ToListAsync(It.IsAny<CancellationToken>()))
                    .Returns((CancellationToken ct) => Task.FromResult(new List<CloudEventOutboxEntry>()));
                return repo.Object;
            });
            CloudEventBusFactory.ConfigureServices(services);
            var provider = services.BuildServiceProvider();
            var cloudEventBus = provider.GetRequiredService<CloudEventBus>();
            var formatter = provider.GetRequiredService<CloudEventFormatter>();
            await cloudEventBus.StartAsync(default);
            var e = CloudEventFactory.Create();
            TestHttpMessageHandler.SendTaskSource = new();

            //act
            await cloudEventBus.PublishAsync(e);
            await TestHttpMessageHandler.SendTaskSource.Task;

            //assert
            outboxEntry.Should().NotBeNull();
            var decoded = await formatter.DecodeStructuredModeMessageAsync(new MemoryStream(outboxEntry.Data), new(outboxEntry.ContentType), null);
            decoded.Should().NotBeNull();
            decoded.Id.Should().Be(e.Id);
            decoded.Source.Should().Be(e.Source);
            decoded.Type.Should().Be(e.Type);
            decoded.Data.Should().NotBeNull();
            var decodedData = decoded.Data.As<JsonElement>().Deserialize(e.Data.GetType());
            decodedData.Should().BeEquivalentTo(e.Data);
        }

        [Fact, Priority(2)]
        public async Task Should_Enqueue_Outbox_OnStart()
        {
            //arrange
            var formatter = new JsonEventFormatter();
            List<CloudEvent> events = new();
            for(int i = 0; i < 10; i++)
            {
                events.Add(CloudEventFactory.Create());
            }
            List<CloudEventOutboxEntry> entries = events.Select(ce => new CloudEventOutboxEntry(ce.Id, formatter.EncodeStructuredModeMessage(ce, out ContentType contentType).ToArray(), contentType)).ToList();
            var services = new ServiceCollection();
            services.AddSingleton(provider =>
            {
                var repo = new Mock<IRepository<CloudEventOutboxEntry, string>>();
                repo.Setup(r => r.ToListAsync(It.IsAny<CancellationToken>()))
                    .Returns((CancellationToken ct) => Task.FromResult(entries));
                return repo.Object;
            });
            CloudEventBusFactory.ConfigureServices(services);
            var provider = services.BuildServiceProvider();
            var cloudEventBus = provider.GetRequiredService<CloudEventBus>();
            var e = CloudEventFactory.Create();
            TestHttpMessageHandler.MessagesSent = 0;

            //act
            await cloudEventBus.StartAsync(default);
            await Task.Delay(50);

            //assert
            TestHttpMessageHandler.MessagesSent.Should().Be(events.Count);
        }

        [Fact, Priority(3)]
        public void Subscribe_Should_Work()
        {
            //arrrange
            var services = new ServiceCollection();
            CloudEventBusFactory.ConfigureServices(services);
            var provider = services.BuildServiceProvider();
            var cloudEventBus = provider.GetRequiredService<CloudEventBus>();
            var stream = provider.GetRequiredService<ISubject<CloudEvent>>();
            var e = CloudEventFactory.Create();

            //act
            bool handled = false;
            using var subscription = cloudEventBus.Subscribe(e => handled = true);
            stream.OnNext(e);

            //assert
            handled.Should().BeTrue();
        }

    }

}
