using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neuroglia.Eventing.Configuration;
using Neuroglia.Eventing.Services;
using Polly;
using Polly.Extensions.Http;
using System.Reactive.Subjects;

namespace Neuroglia.Eventing
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds a new <see cref="CloudEventBus"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="CloudEventBus"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCloudEventBus(this IServiceCollection services, Action<ICloudEventBusOptionsBuilder> setup)
        {
            var builder = new CloudEventBusOptionsBuilder();
            setup(builder);
            var options = builder.Build();
            services.TryAddSingleton(Options.Create(options));
            services.TryAddSingleton<ISubject<CloudEvent>, Subject<CloudEvent>>();
            services.AddHttpClient(nameof(CloudEventBus), (provider, http) =>
            {
                CloudEventBusOptions options = provider.GetRequiredService<IOptions<CloudEventBusOptions>>().Value;
                http.BaseAddress = options.BrokerUri;
            })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetPolicy());
            services.AddSingleton<CloudEventBus>();
            services.AddSingleton<ICloudEventBus>(provider => provider.GetRequiredService<CloudEventBus>());
            services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<CloudEventBus>());
            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetPolicy()
        {
            var circuitBreakerPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(response => !response.IsSuccessStatusCode)
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(10));
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2,retry)));
            return retryPolicy.WrapAsync(circuitBreakerPolicy);
        }

    }

}
