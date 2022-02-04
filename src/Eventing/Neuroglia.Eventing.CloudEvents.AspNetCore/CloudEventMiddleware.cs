using CloudNative.CloudEvents;
using CloudNative.CloudEvents.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;

namespace Neuroglia.Eventing
{

    /// <summary>
    /// Represents the <see cref="RequestDelegate"/> used to handle <see cref="CloudEvent"/>s
    /// </summary>
    public class CloudEventMiddleware
    {

        /// <summary>
        /// Initializes a new <see cref="CloudEventMiddleware"/>
        /// </summary>
        /// <param name="next">The next <see cref="RequestDelegate"/> in the pipeline</param>
        /// <param name="logger">The service used to perform logging</param>
        public CloudEventMiddleware(RequestDelegate next, ILogger<CloudEventMiddleware> logger)
        {
            this.Next = next;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the next <see cref="RequestDelegate"/> in the pipeline
        /// </summary>
        protected RequestDelegate Next { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task InvokeAsync(HttpContext context, CloudEventFormatter formatter, ISubject<CloudEvent> stream)
        {
            if(!context.Request.IsCloudEvent())
            {
                await this.Next(context);
                return;
            }
            try
            {
                var e = await context.Request.ToCloudEventAsync(formatter, Array.Empty<CloudEventAttribute>());
                stream.OnNext(e);
            }
            catch(Exception ex)
            {
                this.Logger.LogError("An error occured while consuming an incoming event: {ex}", ex.ToString());
            }
        }

    }

}
