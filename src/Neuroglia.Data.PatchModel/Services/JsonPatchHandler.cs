using Json.Patch;
using Neuroglia.Serialization.Json;

namespace Neuroglia.Data.PatchModel.Services;

/// <summary>
/// Represents the <see cref="IPatchHandler"/> implementation used to handle Json Patches
/// </summary>
public class JsonPatchHandler
    : IPatchHandler
{

    /// <inheritdoc/>
    public virtual bool Supports(string type) => type == PatchType.JsonPatch;

    /// <inheritdoc/>
    public virtual Task<T?> ApplyPatchAsync<T>(object patch, T? target, CancellationToken cancellationToken = default)
    {
        var jsonPatch = JsonSerializer.Default.Deserialize<JsonPatch>(JsonSerializer.Default.SerializeToText(patch))!;
        return Task.FromResult(JsonSerializer.Default.Deserialize<T>(jsonPatch.Apply(JsonSerializer.Default.SerializeToNode((object?)target)).Result!));
    }

}
