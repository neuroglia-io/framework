using System;

namespace Neuroglia.Data
{

    /// <summary>
    /// Represents an <see cref="Attribute"/> used to mark a class as an OData entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ODataEntityAttribute
        : Attribute
    {

        /// <summary>
        /// Initializes a new <see cref="ODataEntityAttribute"/>
        /// </summary>
        public ODataEntityAttribute()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="ODataEntityAttribute"/>
        /// </summary>
        /// <param name="collection">The name of the collection the marked class belongs to</param>
        public ODataEntityAttribute(string collection)
        {
            this.Collection = collection;
        }

        /// <summary>
        /// Gets/sets the name of the collection the marked class belongs to
        /// </summary>
        public string Collection { get; set; }

    }

}
