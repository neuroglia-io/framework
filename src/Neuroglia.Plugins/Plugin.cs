using System.Reflection;

namespace Neuroglia.Plugins;

/// <summary>
/// Represents the default implementation of the <see cref="IPlugin"/> interface
/// </summary>
public class Plugin
    : IPlugin
{

    /// <summary>
    /// Initializes a new <see cref="Plugin"/>
    /// </summary>
    protected Plugin() { }

    /// <summary>
    /// Initializes a new <see cref="Plugin"/>
    /// </summary>
    /// <param name="name">The name of the plugin</param>
    /// <param name="version">The version of the plugin</param>
    /// <param name="type">The type of the plugin</param>
    /// <param name="assembly">The assembly of the plugin</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Plugin(string name, Version version, Type type, Assembly assembly)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        this.Name = name;
        this.Version = version ?? new(1, 0, 0);
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    /// <inheritdoc/>
    public virtual string Name { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual Version Version { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual Type Type { get; protected set; } = null!;

    /// <inheritdoc/>
    public virtual Assembly Assembly { get; protected set; } = null!;

}
