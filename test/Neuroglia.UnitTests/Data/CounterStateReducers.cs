using Neuroglia.Data.Flux;

namespace Neuroglia.UnitTests.Data
{

    public static class CounterStateReducers
    {

        [Reducer]
        public static CounterFeature IncrementCounter(CounterFeature counter, IncrementCountAction increment) => new(new CounterState(counter.Counter.Count + increment.Amount));

    }

}