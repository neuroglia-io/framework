using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Neuroglia.Data;

/// <summary>
/// Represents a patch
/// </summary>
[DataContract]
public class Patch
{

    /// <summary>
    /// Initializes a new <see cref="Patch"/>
    /// </summary>
    [JsonConstructor]
    protected Patch() { }

    /// <summary>
    /// Initializes a new <see cref="Patch"/>
    /// </summary>
    /// <param name="type">The patch type</param>
    /// <param name="document">The patch document</param>
    public Patch(string type, object document)
    {
        this.Type = type;
        this.Document = document ?? throw new NullReferenceException(nameof(Document));
    }

    /// <summary>
    /// Gets the patch type
    /// </summary>
    [DataMember(IsRequired = true)]
    public virtual string Type { get; protected set; } = null!;

    /// <summary>
    /// Gets the patch document
    /// </summary>
    [DataMember(IsRequired = true)]
    public virtual object Document { get; protected set; } = null!;

}
