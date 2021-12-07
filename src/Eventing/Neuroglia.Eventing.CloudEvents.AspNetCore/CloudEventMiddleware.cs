using CloudNative.CloudEvents;
using CloudNative.CloudEvents.AspNetCore;
using CloudNative.CloudEvents.Core;
using Microsoft.AspNetCore.Http;
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
        public CloudEventMiddleware(RequestDelegate next)
        {
            this.Next = next;
        }

        /// <summary>
        /// Gets the next <see cref="RequestDelegate"/> in the pipeline
        /// </summary>
        protected RequestDelegate Next { get; }

        /// <inheritdoc/>
        public async Task InvokeAsync(HttpContext context, CloudEventFormatter formatter, ISubject<CloudEvent> cloudEventStream)
        {
            if(!context.Request.IsCloudEvent())
            {
                await this.Next(context);
                return;
            }
            var e = await context.Request.ToCloudEventAsync(formatter, Array.Empty<CloudEventAttribute>());
            cloudEventStream.OnNext(e);
        }

    }

}
