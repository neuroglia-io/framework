using Neuroglia.Data.Flux;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Data
{
    public class TestFluxMiddleware
       : IMiddleware
    {

        public static int ValueBeforeDispatch;

        public static int ValueAfterDispatch;

        public const int Multiplier = 3;

        public TestFluxMiddleware(DispatchDelegate next)
        {
            this.Next = next;
        }

        protected DispatchDelegate Next { get; }

        public virtual async Task<object> InvokeAsync(IActionContext context)
        {
            var result = null as object;
            if(context.Action is IncrementCountAction incrementCount)
            {
                ValueBeforeDispatch = incrementCount.Amount;
                result = await this.Next(context);
                ValueAfterDispatch = incrementCount.Amount;
            }
            return result;
        }

    }

}
