using OpenTelemetry.Trace;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines extensions to configure the Neuroglia <see cref="IMediator"/> OpenTelemetry instrumentation
    /// </summary>
    public static class MediatorOpenTelemetryInstrumentation
    {

        /// <summary>
        /// Adds and configures the <see cref="IMediator"/> OpenTelemetry instrumentation
        /// </summary>
        /// <param name="builder">The <see cref="TracerProviderBuilder"/> to configure</param>
        /// <returns>The configured <see cref="TracerProviderBuilder"/></returns>
        public static TracerProviderBuilder AddMediatorInstrumentation(this TracerProviderBuilder builder)
        {
            return builder.AddSource(Mediator.ActivitySourceName);
        }

    }

}
