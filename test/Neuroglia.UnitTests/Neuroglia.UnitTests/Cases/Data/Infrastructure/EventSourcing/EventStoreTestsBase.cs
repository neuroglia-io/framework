using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.EventSourcing;

public abstract class EventStoreTestsBase
    : IAsyncLifetime
{

    public EventStoreTestsBase(IServiceCollection services)
    {
        ServiceProvider = services.BuildServiceProvider();
    }

    protected ServiceProvider ServiceProvider { get; }

    protected CancellationTokenSource CancellationTokenSource { get; } = new();

    protected IEventStore EventStore { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        foreach (var hostedService in ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(CancellationTokenSource.Token).ConfigureAwait(false);
        }
        EventStore = ServiceProvider.GetRequiredService<IEventStore>();
    }

    public async Task DisposeAsync() => await ServiceProvider.DisposeAsync().ConfigureAwait(false);

    [Fact, Priority(1)]
    public async Task Append_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //act
        await EventStore.AppendAsync(streamId, events);
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, 0).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);
        for (var i = 0; i < storedEvents.Count; i++)
        {
            var originalEvent = events[i];
            var storedEvent = storedEvents[i];
            storedEvent.Id.Should().NotBeNullOrWhiteSpace();
            storedEvent.Timestamp.Should().NotBe(default);
            storedEvent.Type.Should().Be(originalEvent.Type);
            storedEvent.Data.Should().BeEquivalentTo(originalEvent.Data);
            storedEvent.Metadata.Should().BeEquivalentTo(originalEvent.Metadata);
        }
    }

    [Fact, Priority(2)]
    public async Task Append_ToNonExistingStream_Should_Work_StreamNotFoundException()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //act
        await EventStore.AppendAsync(streamId, events, -1);
    }

    [Fact, Priority(3)]
    public async Task Append_WithValidExpectedVersion_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.AppendAsync(streamId, eventsToAppend, events.Count - 1);
    }

    [Fact, Priority(4)]
    public async Task Append_WithInvalidExpectedVersion_Should_Throw_OptmisticConcurrencyException()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //assert
        var action = async () => await EventStore.AppendAsync(streamId, events, events.Count + 10);
        await action.Should().ThrowAsync<OptimisticConcurrencyException>();
    }

    [Fact, Priority(5)]
    public async Task Get_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var stream = await EventStore.GetAsync(streamId);
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, 0).ToListAsync();

        //assert
        stream.Should().NotBeNull();
        stream.Id.Should().Be(streamId);
        stream.Length.Should().Be(events.Count);
        stream.FirstEventAt.Should().Be(storedEvents.First().Timestamp);
        stream.LastEventAt.Should().Be(storedEvents.Last().Timestamp);
    }

    [Fact, Priority(6)]
    public async Task Get_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.GetAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(7)]
    public async Task Read_Forwards_FromStart_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);

        storedEvents.First().Type.Should().Be(events.First().Type);
        storedEvents.First().Data.Should().BeEquivalentTo(events.First().Data);

        storedEvents.Last().Type.Should().Be(events.Last().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.Last().Data);
    }

    [Fact, Priority(8)]
    public async Task Read_Forwards_FromEnd_Should_BeEmpty()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact, Priority(9)]
    public async Task Read_Forwards_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count / 2;
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be((int)(events.Count - offset));
        storedEvents.First().Offset.Should().Be((ulong)offset);

        storedEvents.Last().Type.Should().Be(events.Last().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.Last().Data);
    }

    [Fact, Priority(10)]
    public async Task Read_Backwards_FromEnd_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);

        storedEvents.First().Type.Should().Be(events.Last().Type);
        storedEvents.First().Data.Should().BeEquivalentTo(events.Last().Data);

        storedEvents.Last().Type.Should().Be(events.First().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.First().Data);
    }

    [Fact, Priority(11)]
    public async Task Read_Backwards_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var offset = 2;
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be(events.Count - offset + 1);
        storedEvents.First().Offset.Should().Be((ulong)offset);

        storedEvents.Last().Type.Should().Be(events.First().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.First().Data);
    }

    [Fact, Priority(12)]
    public async Task Read_Backwards_FromStart_Should_BeEmpty()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact, Priority(13)]
    public async Task Read_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.ReadAsync("non-existing", StreamReadDirection.Forwards, 0).ToListAsync();
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(14)]
    public async Task Subscribe_FromStart_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        await EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await EventStore.SubscribeAsync(streamId, StreamPosition.StartOfStream))
            .Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(streamId, eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveCount(events.Count + eventsToAppend.Count);
    }

    [Fact, Priority(15)]
    public async Task Subscribe_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count;
        var handledEvents = new List<IEventRecord>();
        await EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await EventStore.SubscribeAsync(streamId, offset))
            .Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(streamId, eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact, Priority(16)]
    public async Task Subscribe_FromEnd_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        await EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await EventStore.SubscribeAsync(streamId))
            .Subscribe(handledEvents.Add);
        await EventStore.AppendAsync(streamId, eventsToAppend);
        await Task.Delay(100);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact, Priority(17)]
    public async Task Subscribe_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.SubscribeAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(18)]
    public async Task Truncate_ToZero_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.TruncateAsync(streamId);
        IEventStreamDescriptor? stream;
        try
        {
            stream = await EventStore.GetAsync(streamId);
        }
        catch (StreamNotFoundException)
        {
            stream = new EventStreamDescriptor(streamId, 0, null, null);
        }

        //assert
        stream.Length.Should().Be(0);
        stream.FirstEventAt.Should().BeNull();
        stream.LastEventAt.Should().BeNull();
    }

    [Fact, Priority(19)]
    public async Task Truncate_Partially_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var truncateBefore = events.Count / 2;
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.TruncateAsync(streamId, (ulong)truncateBefore);
        var stream = await EventStore.GetAsync(streamId);

        //assert
        stream.Length.Should().Be(truncateBefore);
        stream.FirstEventAt.Should().NotBeNull();
        stream.LastEventAt.Should().NotBeNull();
    }

    [Fact, Priority(20)]
    public async Task Truncate_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.TruncateAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(21)]
    public async Task Delete_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await EventStore.AppendAsync(streamId, events);

        //act
        await EventStore.DeleteAsync(streamId);

        //assert
        var action = async () => await EventStore.GetAsync(streamId);
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact, Priority(22)]
    public async Task Delete_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await EventStore.DeleteAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

}
