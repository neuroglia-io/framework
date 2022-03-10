using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data.Flux;
using Neuroglia.UnitTests.Data;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Flux
{

    public class FluxTests
    {

        [Fact]
        public void Flux_Feature_Should_Work()
        {
            //arrange
            var dispatcher = new Dispatcher();
            var store = new Store(dispatcher);
            var originalCount = 3;
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);
            var feature = new CounterFeature(new(originalCount));
            store.AddFeature(feature, new Reducer<CounterState, IncrementCountAction>(CounterStateReducers.IncrementCounter));

            //act
            dispatcher.Dispatch(action);

            //assert
            feature.CounterState.Count.Should().Be(originalCount + incrementAmount);
        }

        [Fact]
        public async Task Flux_Effect_Should_Apply()
        {
            //arrange
            var dispatcher = new Dispatcher();
            var store = new Store(dispatcher);
            var actionCount = 24;
            var effectCount = -1;
            var action = new IncrementCountAction(actionCount);
            var effect = new Effect<IncrementCountAction>(async (action, context) =>
            {
                effectCount = action.Amount;
                await Task.CompletedTask;
            });
            store.AddEffect(effect);

            //act
            dispatcher.Dispatch(action);
            await Task.Delay(100);

            //assert
            effectCount.Should().Be(actionCount);
        }

        [Fact]
        public void Flux_Middleware_Should_Run()
        {
            //arrange
            var dispatcher = new Dispatcher();
            var store = new Store(dispatcher);
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);
            store.AddFeature<CounterFeature>(new Reducer<CounterState, IncrementCountAction>(CounterStateReducers.IncrementCounter));
            store.AddMiddleware<TestFluxMiddleware>();

            //act
            dispatcher.Dispatch(action);

            //assert
            TestFluxMiddleware.ValueBeforeDispatch.Should().Be(incrementAmount);
            TestFluxMiddleware.ValueAfterDispatch.Should().Be(incrementAmount);
        }

        [Fact]
        public void Flux_Dependency_Injection_Should_Work()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddFlux(flux => flux.ScanMarkupTypeAssembly<FluxTests>());
            var provider = services.BuildServiceProvider();
            var store = provider.GetRequiredService<IStore>();
            var feature = store.GetFeature<CounterFeature>();
            var originalCount = feature.Value.CounterState.Count;
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);
            var dispatcher = provider.GetRequiredService<IDispatcher>();

            //act
            dispatcher.Dispatch(action);

            //assert
            feature.Value.CounterState.Count.Should().Be(originalCount + incrementAmount);
        }

    }

}
