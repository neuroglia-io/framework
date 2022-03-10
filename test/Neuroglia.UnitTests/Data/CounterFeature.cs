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
        this.CounterState = counterState;
    }

    public CounterState CounterState { get; protected set; } = new();

}