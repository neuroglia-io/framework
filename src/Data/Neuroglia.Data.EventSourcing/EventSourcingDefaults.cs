namespace Neuroglia.Data.EventSourcing
{

    /// <summary>
    /// Exposes event sourcing defaults
    /// </summary>
    public static class EventSourcingDefaults
    {

        /// <summary>
        /// Exposes event sourcing metadata defaults
        /// </summary>
        public static class Metadata
        {

            /// <summary>
            /// Gets the prefix of the default event-sourcing related metadata properties
            /// </summary>
            public const string Prefix = "es-meta-";

            /// <summary>
            /// Gets the name of the metadata used to store the assembly-qualified name of the serialized event's runtime type
            /// </summary>
            public const string RuntimeTypeName = Prefix + "runtime-type";

        }

    }

}
