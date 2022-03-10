using Neuroglia.Data.Flux;

namespace Neuroglia.UnitTests.Data
{

    public static class CounterStateReducers
    {

        [Reducer]
        public static CounterState IncrementCounter(CounterState counter, IncrementCountAction increment) => new(counter.Count + increment.Amount);

    }

}