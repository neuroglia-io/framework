using Neuroglia.Data.Infrastructure.EventSourcing;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;

namespace Neuroglia.UnitTests.Cases.EventSourcing;

public abstract class EventStoreTestsBase
{

    public EventStoreTestsBase(IEventStore eventStore)
    {
        this.EventStore = eventStore;
    }

    protected IEventStore EventStore { get; }

    [Fact]
    public async Task Append_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //act
        await this.EventStore.AppendAsync(streamId, events);
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, 0).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);
        for(var i = 0; i < storedEvents.Count; i++)
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

    [Fact]
    public async Task Append_ToNonExistingStream_Should_Work_StreamNotFoundException()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //act
        await this.EventStore.AppendAsync(streamId, events, -1);
    }

    [Fact]
    public async Task Append_WithValidExpectedVersion_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        await this.EventStore.AppendAsync(streamId, eventsToAppend, events.Count);
    }

    [Fact]
    public async Task Append_WithInvalidExpectedVersion_Should_Throw_OptmisticConcurrencyException()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();

        //assert
        var action = async () => await this.EventStore.AppendAsync(streamId, events, events.Count + 10);
        await action.Should().ThrowAsync<OptimisticConcurrencyException>();
    }

    [Fact]
    public async Task Get_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var stream = await this.EventStore.GetAsync(streamId);
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, 0).ToListAsync();

        //assert
        stream.Should().NotBeNull();
        stream.Id.Should().Be(streamId);
        stream.Length.Should().Be(events.Count);
        stream.FirstEventAt.Should().Be(storedEvents.First().Timestamp);
        stream.LastEventAt.Should().Be(storedEvents.Last().Timestamp);
    }

    [Fact]
    public async Task Get_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await this.EventStore.GetAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact]
    public async Task Read_Forwards_FromStart_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);

        storedEvents.First().Type.Should().Be(events.First().Type);
        storedEvents.First().Data.Should().Be(events.First().Data);

        storedEvents.Last().Type.Should().Be(events.Last().Type);
        storedEvents.Last().Data.Should().Be(events.Last().Data);
    }

    [Fact]
    public async Task Read_Forwards_FromEnd_Should_BeEmpty()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task Read_Forwards_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count() / 2;
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Forwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be((int)(events.Count() - offset));
        storedEvents.First().Offset.Should().Be((ulong)offset);

        storedEvents.Last().Type.Should().Be(events.Last().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.Last().Data);
    }

    [Fact]
    public async Task Read_Backwards_FromEnd_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, StreamPosition.EndOfStream).ToListAsync();

        //assert
        storedEvents.Should().HaveSameCount(events);

        storedEvents.First().Type.Should().Be(events.Last().Type);
        storedEvents.First().Data.Should().BeEquivalentTo(events.Last().Data);

        storedEvents.Last().Type.Should().Be(events.First().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.First().Data);
    }

    [Fact]
    public async Task Read_Backwards_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count / 2;
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, offset).ToListAsync();

        //assert
        storedEvents.Count.Should().Be((int)(events.Count() - offset));
        storedEvents.First().Offset.Should().Be((ulong)offset - 1);

        storedEvents.Last().Type.Should().Be(events.First().Type);
        storedEvents.Last().Data.Should().BeEquivalentTo(events.First().Data);
    }

    [Fact]
    public async Task Read_Backwards_FromEnd_Should_BeEmpty()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var storedEvents = await this.EventStore.ReadAsync(streamId, StreamReadDirection.Backwards, StreamPosition.StartOfStream).ToListAsync();

        //assert
        storedEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task Read_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await this.EventStore.ReadAsync("non-existing", StreamReadDirection.Forwards, 0).ToListAsync();
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact]
    public async Task Subscribe_FromStart_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await this.EventStore.SubscribeAsync(streamId, StreamPosition.StartOfStream))
            .Subscribe(handledEvents.Add);
        await this.EventStore.AppendAsync(streamId, eventsToAppend);

        //assert
        handledEvents.Should().HaveCount(events.Count + eventsToAppend.Count);
    }

    [Fact]
    public async Task Subscribe_FromOffset_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var offset = (long)events.Count;
        var handledEvents = new List<IEventRecord>();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await this.EventStore.SubscribeAsync(streamId, offset))
            .Subscribe(handledEvents.Add);
        await this.EventStore.AppendAsync(streamId, eventsToAppend);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact]
    public async Task Subscribe_FromEnd_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var eventsToAppend = EventStreamFactory.Create().ToList();
        var handledEvents = new List<IEventRecord>();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        var observable = (await this.EventStore.SubscribeAsync(streamId))
            .Subscribe(handledEvents.Add);
        await this.EventStore.AppendAsync(streamId, eventsToAppend);

        //assert
        handledEvents.Should().HaveSameCount(eventsToAppend);
    }

    [Fact]
    public async Task Subscribe_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await this.EventStore.SubscribeAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact]
    public async Task Truncate_ToZero_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        await this.EventStore.TruncateAsync(streamId);
        var stream = await this.EventStore.GetAsync(streamId);

        //assert
        stream.Length.Should().Be(0);
        stream.FirstEventAt.Should().BeNull();
        stream.LastEventAt.Should().BeNull();
    }

    [Fact]
    public async Task Truncate_Partially_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        var truncateBefore = events.Count / 2;
        await this.EventStore.AppendAsync(streamId, events);

        //act
        await this.EventStore.TruncateAsync(streamId, (ulong)truncateBefore);
        var stream = await this.EventStore.GetAsync(streamId);

        //assert
        stream.Length.Should().Be(truncateBefore);
        stream.FirstEventAt.Should().NotBeNull();
        stream.LastEventAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Truncate_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await this.EventStore.TruncateAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact]
    public async Task Delete_Should_Work()
    {
        //arrange
        var streamId = "fake-stream";
        var events = EventStreamFactory.Create().ToList();
        await this.EventStore.AppendAsync(streamId, events);

        //act
        await this.EventStore.DeleteAsync(streamId);

        //assert
        var action = async () => await this.EventStore.GetAsync(streamId);
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

    [Fact]
    public async Task Delete_NonExisting_Should_Throw_StreamNotFoundException()
    {
        //assert
        var action = async () => await this.EventStore.DeleteAsync("non-existing");
        await action.Should().ThrowAsync<StreamNotFoundException>();
    }

}
