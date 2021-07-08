using System.Collections.Generic;

namespace Neuroglia.AspNetCore.Configuration
{

    /// <summary>
    /// Represents the options used to configure header propagation
    /// </summary>
    public class HeaderPropagationOptions
    {

        /// <summary>
        /// Gets/sets a boolean indicating whether or not to propagate all headers
        /// </summary>
        public virtual bool PropagateAll { get; set; } = false;

        /// <summary>
        /// Gets/sets a <see cref="List{T}"/> containing the names of the headers to propagate
        /// </summary>
        public virtual HashSet<string> Headers { get; set; } = new();

    }

}
