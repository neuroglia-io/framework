namespace Neuroglia.Serialization
{
    /// <summary>
    /// Enumerates all supported proto types
    /// </summary>
    public enum ProtoType
    {
        /// <summary>
        /// Represents a null value
        /// </summary>
        Empty,
        /// <summary>
        /// Represents a char or a string
        /// </summary>
        String,
        /// <summary>
        /// Represents a boolean
        /// </summary>
        Boolean,
        /// <summary>
        /// Represents a timestamp
        /// </summary>
        Timestamp,
        /// <summary>
        /// Represents a duration
        /// </summary>
        Duration,
        /// <summary>
        /// Represents an integer number
        /// </summary>
        Integer,
        /// <summary>
        /// Represents an long number
        /// </summary>
        Long,
        /// <summary>
        /// Represents a double, a float or a decimal
        /// </summary>
        Double,
        /// <summary>
        /// Represents an array
        /// </summary>
        Array,
        /// <summary>
        /// Represents a complex object
        /// </summary>
        Object
    }

}
