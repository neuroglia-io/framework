using Neuroglia.Data.Flux;
using Neuroglia.UnitTests.Data;

[Feature]
public class CounterFeature
{

    public CounterFeature()
    {

    }

    public CounterFeature(CounterState counterState)
    {
        this.Counter = counterState;
    }

    public CounterState Counter { get; protected set; } = new();

}