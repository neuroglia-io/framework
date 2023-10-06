using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to serialize ISO 8601 <see cref="Enum"/>s
/// </summary>
public class StringEnumSerializer
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => type.IsEnum;

    /// <inheritdoc/>
    public virtual object ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type) => emitter.Emit(new Scalar(EnumHelper.Stringify((Enum)value!, type)));

}