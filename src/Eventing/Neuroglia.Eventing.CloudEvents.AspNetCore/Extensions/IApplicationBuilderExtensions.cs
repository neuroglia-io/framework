using Microsoft.AspNetCore.Builder;

namespace Neuroglia.Eventing
{

    /// <summary>
    /// Defines extensions for <see cref="IApplicationBuilder"/>s
    /// </summary>
    public static class IApplicationBuilderExtensions
    {

        /// <summary>
        /// Configures the <see cref="IApplicationBuilder"/> to use the <see cref="CloudEventMiddleware"/>
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure</param>
        /// <returns>The configured <see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseCloudEvents(this IApplicationBuilder app)
        {
            app.UseMiddleware<CloudEventMiddleware>();
            return app;
        }

    }

}
