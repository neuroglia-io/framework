using Neuroglia.Data.Flux;

namespace Neuroglia.UnitTests.Data
{
    public class TestFluxMiddleware
       : IMiddleware
    {

        public static int ValueBeforeDispatch;

        public static int ValueAfterDispatch;

        public const int Multiplier = 3;

        public void OnDispatching(object action)
        {
            if (action is IncrementCountAction test)
                ValueBeforeDispatch = test.Amount;
        }

        public void OnDispatched(object action)
        {
            if (action is IncrementCountAction test)
                ValueAfterDispatch = test.Amount;
        }

    }

}
