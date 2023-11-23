namespace Neuroglia;

/// <summary>
/// Represents the attribute used to mark a type as the version that should a specific type should be migrated to
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MigrateFromAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="MigrateFromAttribute"/>
    /// </summary>
    /// <param name="sourceType">The type that should be migrated to the marked class</param>
    public MigrateFromAttribute(Type sourceType) => this.SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));

    /// <summary>
    /// Gets the type that should be migrated to the marked class
    /// </summary>
    public virtual Type SourceType { get; }

}