using Microsoft.AspNetCore.JsonPatch;

namespace Neuroglia.Data
{

    /// <summary>
    /// Defines the fundamentals of an object that can be patched
    /// </summary>
    public interface IPatchable
    {

        /// <summary>
        /// Attempts to get the current <see cref="JsonPatchDocument"/>
        /// </summary>
        /// <param name="patch">The current <see cref="JsonPatchDocument"/>, if any</param>
        /// <returns>A boolean indicating whether or not the <see cref="IPatchable"/> has a pending patch</returns>
        bool TryGetPatch(out JsonPatchDocument patch);

    }

}
