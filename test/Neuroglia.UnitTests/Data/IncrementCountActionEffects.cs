using Neuroglia.Data.Flux;
using System.Threading.Tasks;

namespace Neuroglia.UnitTests.Data
{

    public static class IncrementCountActionEffects
    {

        [Effect]
        public static async Task IncrementCountEffect(IncrementCountAction increment)
        {
            await Task.CompletedTask;
        }

    }

}
