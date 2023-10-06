using Neuroglia.Serialization.Json;

namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Represents the <see cref="IPatchHandler"/> implementation used to handle Json Strategic Merge Patches
/// </summary>
public class JsonStrategicMergePatchHandler
    : IPatchHandler
{

    /// <inheritdoc/>
    public virtual bool Supports(string type) => type == PatchType.JsonStrategicMergePatch;

    /// <inheritdoc/>
    public virtual Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default)
    {
        var targetNode = JsonSerializer.Default.SerializeToNode((object?)patch)!;
        var patchNode = JsonSerializer.Default.SerializeToNode(patch)!;
        return Task.FromResult(JsonSerializer.Default.Deserialize<T?>(JsonStrategicMergePatch.ApplyPatch(targetNode, patchNode)!));
    }

}