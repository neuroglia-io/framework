using Neuroglia.Data.Flux;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Data
{
    public class MultiplierFluxMiddleware
       : IMiddleware
    {

        public const int Multiplier = 3;

        public MultiplierFluxMiddleware(DispatchDelegate next)
        {
            this.Next = next;
        }

        protected DispatchDelegate Next { get; }

        public virtual async Task<object> InvokeAsync(IActionContext context)
        {
            var result = await this.Next(context);
            if (result is CounterFeature counterFeature)
                return new CounterFeature(new(counterFeature.Counter.Count * Multiplier));
            else
                return result;
        }

    }

}
