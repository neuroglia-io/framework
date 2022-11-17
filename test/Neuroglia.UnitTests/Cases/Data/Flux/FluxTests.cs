using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Moq;
using Neuroglia.Data.Flux;
using Neuroglia.Serialization;
using Neuroglia.UnitTests.Data;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Data.Flux
{

    public class FluxTests
    {

        [Fact]
        public async Task Flux_Feature_Should_Work()
        {
            //arrange
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dispatcher = new Dispatcher();
            var store = new Store(serviceProvider, new NullLogger<Store>(), dispatcher);
            var originalCount = 3;
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);
            store.AddFeature(new CounterFeature(new(originalCount)), new Reducer<CounterFeature, IncrementCountAction>(CounterStateReducers.IncrementCounter));

            //act
            dispatcher.Dispatch(action);
            await Task.Delay(1);

            //assert
            store.GetFeature<CounterFeature>().State.Counter.Count.Should().Be(originalCount + incrementAmount);
        }

        [Fact]
        public async Task Flux_Effect_Should_Apply()
        {
            //arrange
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dispatcher = new Dispatcher();
            var store = new Store(serviceProvider, new NullLogger<Store>(), dispatcher);
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
            await Task.Delay(1);

            //assert
            effectCount.Should().Be(actionCount);
        }

        [Fact]
        public async Task Flux_Middleware_Should_Run()
        {
            //arrange
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var dispatcher = new Dispatcher();
            var store = new Store(serviceProvider, new NullLogger<Store>(), dispatcher);
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);
            store.AddFeature<CounterFeature>(new Reducer<CounterFeature, IncrementCountAction>(CounterStateReducers.IncrementCounter));
            store.AddMiddleware<MultiplierFluxMiddleware>();

            //act
            dispatcher.Dispatch(action);
            await Task.Delay(1);
            var feature = store.GetFeature<CounterFeature>();

            //assert
            feature.Should().NotBeNull();
            feature.State.Counter.Count.Should().Be(incrementAmount * MultiplierFluxMiddleware.Multiplier);
        }

        [Fact]
        public async Task Flux_Dependency_Injection_Should_Work()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddFlux(flux => flux.ScanMarkupTypeAssembly<FluxTests>());
            var provider = services.BuildServiceProvider();
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            var store = provider.GetRequiredService<IStore>();
            var originalCount = store.GetFeature<CounterFeature>().State.Counter.Count;
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);

            //act
            dispatcher.Dispatch(action);
            var feature = store.GetFeature<CounterFeature>();

            await Task.Delay(1); //todo: why on earth Flux with DI is the only case where we have to wait a bit before getting the updated state???

            //assert
            feature.Should().NotBeNull();
            feature.State.Counter.Count.Should().Be(originalCount + incrementAmount);
        }

        [Fact]
        public async Task Flux_With_Redux_Dev_Tools_Should_Work()
        {
            //arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(provider =>
            {
                return new Mock<IJSRuntime>().Object;
            });
            services.AddNewtonsoftJsonSerializer();
            services.AddFlux(flux => flux.ScanMarkupTypeAssembly<FluxTests>().UseReduxDevTools());
            var provider = services.BuildServiceProvider();
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            var store = provider.GetRequiredService<IStore>();
            var originalCount = store.GetFeature<CounterFeature>().State.Counter.Count;
            var incrementAmount = 7;
            var action = new IncrementCountAction(incrementAmount);

            //act
            dispatcher.Dispatch(action);
            var feature = store.GetFeature<CounterFeature>();

            await Task.Delay(100); //todo: why on earth Flux with DI is the only case where we have to wait a bit before getting the updated state???

            //assert
            feature.Should().NotBeNull();
            feature.State.Counter.Count.Should().Be(originalCount + incrementAmount);
        }

    }

}
