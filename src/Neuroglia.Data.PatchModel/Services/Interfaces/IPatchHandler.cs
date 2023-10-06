namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Defines the fundamentals of a service used to apply patches of a specific type
/// </summary>
public interface IPatchHandler
{

    /// <summary>
    /// Indicates whether or not the handler supports the specified patch type
    /// </summary>
    /// <param name="type">The type of patch to handle</param>
    /// <returns>A boolean indicating whether or not the handler supports the specified patch type</returns>
    bool Supports(string type);

    /// <summary>
    /// Applies a <see cref="Patch"/> to the specified target
    /// </summary>
    /// <typeparam name="T">The type of the object to apply the patch to</typeparam>
    /// <param name="patch">The <see cref="Patch"/> to apply</param>
    /// <param name="target">The object to apply the patch to</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The patched target</returns>
    Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default);

}