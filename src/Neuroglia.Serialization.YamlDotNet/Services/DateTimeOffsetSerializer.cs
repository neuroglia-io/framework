using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Neuroglia.Serialization.Yaml;

/// <summary>
/// Represents the <see cref="IYamlTypeConverter"/> used to serialize <see cref="DateTimeOffset"/>s
/// </summary>
public class DateTimeOffsetSerializer
    : IYamlTypeConverter
{

    /// <inheritdoc/>
    public virtual bool Accepts(Type type) => typeof(DateTimeOffset).IsAssignableFrom(type);

    /// <inheritdoc/>
    public virtual object ReadYaml(IParser parser, Type type)
    {
        Scalar scalar = (Scalar)parser.Current!;
        parser.MoveNext();
        return DateTimeOffset.Parse(scalar.Value, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    public virtual void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value == null || value is not DateTimeOffset dateTime) return;
        emitter.Emit(new Scalar(dateTime.ToString("o", CultureInfo.InvariantCulture)));
    }

}