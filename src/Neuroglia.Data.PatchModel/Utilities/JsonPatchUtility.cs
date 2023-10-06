using Json.Patch;
using Neuroglia.Serialization.Json;

namespace Neuroglia.Data;

/// <summary>
/// Exposes methods to help handling <see cref="JsonPatch"/>es
/// </summary>
public static class JsonPatchUtility
{

    /// <summary>
    /// Creates a new <see cref="JsonPatch"/> based on the differences between the specified values
    /// </summary>
    /// <param name="source">The source object</param>
    /// <param name="target">The target object</param>
    /// <returns>A new <see cref="JsonPatch"/> based on the differences between the specified values</returns>
    public static JsonPatch CreateJsonPatchFromDiff<T>(object? source, object? target)
    {
        source ??= new();
        target ??= new();
        var sourceToken = JsonSerializer.Default.SerializeToElement(source)!.Value;
        var targetToken = JsonSerializer.Default.SerializeToElement(target)!.Value;
        var patchDocument = JsonCons.Utilities.JsonPatch.FromDiff(sourceToken, targetToken);
        return JsonSerializer.Default.Deserialize<JsonPatch>(patchDocument.RootElement)!;
    }

}