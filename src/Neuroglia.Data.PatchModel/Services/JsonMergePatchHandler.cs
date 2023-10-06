using Neuroglia.Serialization.Json;

namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Represents the <see cref="IPatchHandler"/> implementation used to handle Json Merge Patches
/// </summary>
public class JsonMergePatchHandler
    : IPatchHandler
{

    /// <inheritdoc/>
    public virtual bool Supports(string type) => type == PatchType.JsonMergePatch;

    /// <inheritdoc/>
    public virtual Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default)
    {
        var targetElement = JsonSerializer.Default.SerializeToElement((object?)target)!.Value;
        var patchElement = JsonSerializer.Default.SerializeToElement(patch)!.Value;
        var updatedDocument = JsonCons.Utilities.JsonMergePatch.ApplyMergePatch(targetElement, patchElement);
        return Task.FromResult(JsonSerializer.Default.Deserialize<T?>(JsonSerializer.Default.SerializeToNode(updatedDocument)!));
    }

}
