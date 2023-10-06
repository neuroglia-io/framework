using System.Reflection;

namespace Neuroglia;

/// <summary>
/// Defines extension methods for <see cref="MemberInfo"/>s
/// </summary>
public static class MemberInfoExtensions
{

    /// <summary>
    /// Attempts to get a custom attribute of the specified <see cref="MemberInfo"/>
    /// </summary>
    /// <typeparam name="TAttribute">The type of the custom attribute to get</typeparam>
    /// <param name="extended">The extended <see cref="MemberInfo"/></param>
    /// <param name="attribute">The resulting custom attribute</param>
    /// <returns>A boolean indicating whether or not the custom attribute of the specified <see cref="MemberInfo"/> could be found</returns>
    public static bool TryGetCustomAttribute<TAttribute>(this MemberInfo extended, out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = extended.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    /// <summary>
    /// Gets the <see cref="MemberInfo"/> overriden by the <see cref="MemberInfo"/>
    /// </summary>
    /// <param name="extended">The <see cref="MemberInfo"/> to get the overriden <see cref="MemberInfo"/> for</param>
    /// <returns>The <see cref="MemberInfo"/> overriden by the <see cref="MemberInfo"/>, if any</returns>
    public static MemberInfo GetOverridenMember(this MemberInfo extended)
    {
        if (extended == null) throw new ArgumentNullException(nameof(extended));
        var declaringType = extended.DeclaringType!;
        if (declaringType == extended.ReflectedType) declaringType = declaringType.GetDeclaringTypeOf(extended);
        return extended switch
        {
            ConstructorInfo constructor => extended,//nothing to do here: constructors cannot be overriden
            FieldInfo field => declaringType.GetField(field.Name)!,
            PropertyInfo property => declaringType.GetProperty(property.Name)!,
            MethodInfo method => declaringType.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray())!,
            _ => throw new NotSupportedException($"The specified member type '{extended.MemberType}' is not supported"),
        };
    }

}