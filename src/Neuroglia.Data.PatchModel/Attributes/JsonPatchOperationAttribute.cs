using Json.Patch;

namespace Neuroglia.Data.PatchModel.Attributes;

/// <summary>
/// Represents an attribute used to marked methods as Json Patch reducers
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class JsonPatchOperationAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="JsonPatchOperationAttribute"/>
    /// </summary>
    /// <param name="type">The type of the supported Json Patch operation</param>
    /// <param name="path">The supported Json Patch path</param>
    public JsonPatchOperationAttribute(string type, string path)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
        this.Type = type;
        this.Path = path;
    }

    /// <summary>
    /// Initializes a new <see cref="JsonPatchOperationAttribute"/>
    /// </summary>
    /// <param name="type">The type of the supported Json Patch operation</param>
    /// <param name="path">The supported Json Patch path</param>
    public JsonPatchOperationAttribute(OperationType type, string path) : this(type.ToString().ToLower(), path) { }

    /// <summary>
    /// Gets the type of the supported Json Patch operation
    /// </summary>
    public virtual string Type { get; }

    /// <summary>
    /// Gets the supported Json Patch path
    /// </summary>
    public virtual string Path { get; }

    /// <summary>
    /// Gets the type of object referenced by the supplied Json Patch value, if any
    /// </summary>
    public virtual Type? ReferencedType { get; init; }

}
