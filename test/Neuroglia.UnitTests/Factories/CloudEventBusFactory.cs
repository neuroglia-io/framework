using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neuroglia.Eventing;
using Neuroglia.Eventing.Configuration;
using Neuroglia.Eventing.Services;
using Neuroglia.UnitTests.Services;
using System;
using System.Reactive.Subjects;

namespace Neuroglia.UnitTests.Factories
{

    internal static class CloudEventBusFactory
    {

        internal static CloudEventBus Create(Action<IServiceCollection> serviceConfiguration)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            if (serviceConfiguration != null)
                serviceConfiguration(services);
            return services.BuildServiceProvider().GetRequiredService<CloudEventBus>();
        }

        internal static CloudEventBus Create()
        {
            return Create(null);
        }

        internal static void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CloudEventBusOptions>(options =>
            {
                options.BrokerUri = new("https://webhook.site/df070939-36f0-49d9-828c-4f182206d852");
            });
            services.AddSingleton<CloudEventFormatter, JsonEventFormatter>();
            services.AddHttpClient<CloudEventBus>((provider, http) =>
            {
                CloudEventBusOptions options = provider.GetRequiredService<IOptions<CloudEventBusOptions>>().Value;
                http.BaseAddress = options.BrokerUri;
            })
                .AddHttpMessageHandler(() => new TestHttpMessageHandler());
            services.AddCloudEventBus(provider => provider.WithBrokerUri(new("https://webhook.site/6ddc1d0a-562b-4e20-bfdc-36293957dab1")));
        }

    }

}
