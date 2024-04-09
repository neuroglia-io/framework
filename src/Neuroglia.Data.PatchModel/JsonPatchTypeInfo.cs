// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Json.Patch;
using Json.Pointer;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Collections;
using Neuroglia.Data.Guards;
using Neuroglia.Data.Infrastructure.Services;
using Neuroglia.Serialization.Json;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Neuroglia.Data.PatchModel;

/// <summary>
/// Represents an object used to store JSON Patch related information about a type
/// </summary>
public class JsonPatchTypeInfo
{

    static readonly ConcurrentDictionary<Type, JsonPatchTypeInfo> Registry = new();

    readonly List<JsonPatchPropertyInfo> _properties = new();
    /// <summary>
    /// Gets a list containing the type's properties
    /// </summary>
    public virtual IReadOnlyCollection<JsonPatchPropertyInfo> Properties => _properties.AsReadOnly();

    /// <summary>
    /// Gets the <see cref="JsonPatchPropertyInfo"/> at the specified path, if any
    /// </summary>
    /// <param name="path">The path of the <see cref="JsonPatchPropertyInfo"/> to get</param>
    /// <returns>The <see cref="JsonPatchPropertyInfo"/> at the specified path, if any</returns>
    public virtual JsonPatchPropertyInfo? GetProperty(JsonPointer path)
    {
        var pathStr = path.ToString();
        if (int.TryParse(path.Segments.Last().Value, out var index)) pathStr = JsonPointer.Create(path.Segments[..^1]).ToString();
        return Properties.FirstOrDefault(p => p.Path.Equals(pathStr, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the an object's property
    /// </summary>
    /// <param name="provider">The current <see cref="IServiceProvider"/></param>
    /// <param name="source">The source object to get the property value of</param>
    /// <param name="path">The path to the property to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The value of an object's property</returns>
    public virtual async Task<object?> GetValueAsync(IServiceProvider provider, object source, JsonPointer path, CancellationToken cancellationToken = default)
    {
        var property = GetProperty(path) ?? throw new NullReferenceException($"Failed to find a property at path '{path.ToString(JsonPointerStyle.Plain)}'");
        return await property.GetValueAsync(provider, source, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets an object's property
    /// </summary>
    /// <param name="provider">The current <see cref="IServiceProvider"/></param>
    /// <param name="source">The source object to get the property value of</param>
    /// <param name="path"></param>
    /// <param name="value">The path to the property to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public virtual async Task SetValueAsync(IServiceProvider provider, object source, JsonPointer path, object? value, CancellationToken cancellationToken = default)
    {
        var property = GetProperty(path) ?? throw new NullReferenceException($"Failed to find a property at path '{path.ToString(JsonPointerStyle.Plain)}'");
        if (property.ReplaceAsync == null) throw new NullReferenceException($"The '{nameof(JsonPatchPropertyInfo.ReplaceAsync)}' delegate has not been configured for property at '{path}'");
        await property.ReplaceAsync(provider, source, PatchOperation.Replace(path, value == null ? null : JsonSerializer.Default.SerializeToNode(value)), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets or creates the <see cref="JsonPatchTypeInfo"/> for the specified type
    /// </summary>
    /// <param name="type">The type to get or create the <see cref="JsonPatchTypeInfo"/> of</param>
    /// <returns>The <see cref="JsonPatchTypeInfo"/> for the specified type</returns>
    public static JsonPatchTypeInfo GetOrCreate(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (Registry.TryGetValue(type, out var typeInfo) && typeInfo != null) return typeInfo;

        typeInfo = new JsonPatchTypeInfo();

        var propertyContainerType = type;
        if (typeof(IAggregateRoot).IsAssignableFrom(type)) propertyContainerType = type.GetGenericType(typeof(IAggregateRoot<,>))?.GenericTypeArguments[1] ?? ((IAggregateRoot)Activator.CreateInstance(type, true)!).State.GetType();
        foreach (var property in propertyContainerType.GetProperties().Where(p => p.CanRead))
        {
            var propertyInfo = new JsonPatchPropertyInfo($"/{property.Name.ToCamelCase()}", property.PropertyType, BuildGetValueDelegateFor(property));
            if (property.GetSetMethod() != null)
            {
                propertyInfo.AddAsync = BuildAddOperationDelegateFor(property);
                propertyInfo.CopyAsync = BuildCopyOperationDelegateFor(property, typeInfo);
                propertyInfo.MoveAsync = BuildMoveOperationDelegateFor(property, typeInfo);
                propertyInfo.RemoveAsync = BuildRemoveOperationDelegateFor(property);
                propertyInfo.ReplaceAsync = BuildReplaceOperationDelegateFor(property);
                propertyInfo.TestAsync = BuildTestOperationDelegateFor(property);
            }
            typeInfo._properties.Add(propertyInfo);
        }

        foreach (var method in type.GetMethods().Where(m => m.TryGetCustomAttribute<JsonPatchOperationAttribute>(out _)))
        {
            var jsonPatchOperationAttribute = method.GetCustomAttribute<JsonPatchOperationAttribute>()!;
            if (!PropertyPath.TryParse(jsonPatchOperationAttribute.Path.Replace('/', '.'), out var propertyPath) || propertyPath == null) throw new Exception($"Failed to parse the specified path '{jsonPatchOperationAttribute.Path}' into a new property path for type '{type.Name}'");
            var parameterExpression = Expression.Parameter(propertyContainerType);
            var memberExpression = propertyPath.ToExpression(parameterExpression);
            var lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);
            var getValueDelegate = lambdaExpression.Compile();
            var property = (PropertyInfo)memberExpression.Member;
            var propertyInfo = typeInfo.GetProperty(JsonPointer.Parse($"/{jsonPatchOperationAttribute.Path}"));
            if (propertyInfo == null)
            {
                propertyInfo = new JsonPatchPropertyInfo($"/{jsonPatchOperationAttribute.Path}", property.PropertyType, BuildGetValueDelegateFor(getValueDelegate));
                typeInfo._properties.Add(propertyInfo);
            }
            foreach (var operationType in EnumHelper.GetFlags(jsonPatchOperationAttribute.Type))
            {
                switch (operationType)
                {
                    case JsonPatchOperationType.Add:
                        propertyInfo.AddAsync = BuildAddOperationDelegateFor(method, jsonPatchOperationAttribute);
                        break;
                    case JsonPatchOperationType.Copy:
                        propertyInfo.CopyAsync = BuildCopyOperationDelegateFor(method, typeInfo);
                        break;
                    case JsonPatchOperationType.Move:
                        propertyInfo.MoveAsync = BuildMoveOperationDelegateFor(method, typeInfo);
                        break;
                    case JsonPatchOperationType.Remove:
                        propertyInfo.RemoveAsync = BuildRemoveOperationDelegateFor(method);
                        break;
                    case JsonPatchOperationType.Replace:
                        propertyInfo.ReplaceAsync = BuildReplaceOperationDelegateFor(method, jsonPatchOperationAttribute);
                        break;
                    case JsonPatchOperationType.Test:
                        BuildTestOperationDelegateFor(getValueDelegate);
                        break;
                    default: throw new NotSupportedException($"The specified {nameof(OperationType)} '{operationType}' is not supported");
                }
            }
        }

        Registry.AddOrUpdate(type, typeInfo, (key, existing) => typeInfo);
        return typeInfo;
    }

    /// <summary>
    /// Gets or creates the <see cref="JsonPatchTypeInfo"/> for the specified type
    /// </summary>
    /// <typeparam name="T">The type to get or create the <see cref="JsonPatchTypeInfo"/> of</typeparam>
    /// <returns>The <see cref="JsonPatchTypeInfo"/> for the specified type</returns>
    public static JsonPatchTypeInfo GetOrCreate<T>() => GetOrCreate(typeof(T));

    /// <summary>
    /// Builds a new delegate for getting the value of the specified <see cref="PropertyInfo"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to build the delegate for</param>
    /// <returns>A new delegate for getting the value of the specified <see cref="PropertyInfo"/></returns>
    public static Func<IServiceProvider, object, CancellationToken, Task<object?>> BuildGetValueDelegateFor(PropertyInfo property) => (provider, source, cancellationToken) => Task.FromResult(property.GetValue(source is IAggregateRoot aggregate ? aggregate.State : source));

    /// <summary>
    /// Builds a new delegate for getting the value of the specified <see cref="PropertyInfo"/>
    /// </summary>
    /// <param name="getValueDelegate">The <see cref="Delegate"/> to build the delegate for</param>
    /// <returns>A new delegate for getting the value of the specified <see cref="PropertyInfo"/></returns>
    public static Func<IServiceProvider, object, CancellationToken, Task<object?>> BuildGetValueDelegateFor(Delegate getValueDelegate) => (provider, source, cancellationToken) => Task.FromResult(getValueDelegate.GetMethodInfo().Invoke(source is IAggregateRoot aggregate ? aggregate.State : source, Array.Empty<object>()));

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Add"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to create the delegate for</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Add"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildAddOperationDelegateFor(PropertyInfo property)
    {
        return (provider, source, operation, cancellationToken) =>
        {
            return Task.Run(() =>
            {
                var propertySource = source is IAggregateRoot aggregate ? aggregate.State : source;
                var sourceValue = property.GetValue(propertySource);
                var valueToAdd = operation.Value == null ? null : JsonSerializer.Default.Deserialize(operation.Value, property.PropertyType)!;
                if (int.TryParse(operation.Path.Segments.Last().Value, out var index))
                {
                    if (property.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) throw new NotSupportedException($"Cannot index a value of type '{property.PropertyType.Name}'");
                    if (sourceValue == null) throw new ArgumentOutOfRangeException($"The target property '{property.Name}' is null and cannot be indexed", nameof(index));
                    if (sourceValue is IList list) list.Insert(index, valueToAdd);
                    else throw new InvalidOperationException($"The target property '{property.Name}' is not a list type and it cannot be added/inserted items");
                }
                else if (sourceValue is ICollection collection && valueToAdd != null) collection.Add(valueToAdd);
                else
                {
                    property.SetValue(propertySource, valueToAdd);
                    return;
                }
            }, cancellationToken);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Add"/>
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create the delegate for</param>
    /// <param name="jsonPatchOperationAttribute">The specified <see cref="MethodInfo"/>'s <see cref="JsonPatchOperationAttribute"/></param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Add"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildAddOperationDelegateFor(MethodInfo method, JsonPatchOperationAttribute jsonPatchOperationAttribute)
    {
        var parameters = method.GetParameters();
        if (parameters.Length < 1 || parameters.Length > 2) throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Add}' must declare a 'value' parameter and optionally an 'index' parameter of type 'int'");
        return async (provider, source, operation, cancellationToken) =>
        {
            object[] args;
            if (parameters.Length == 1 && parameters.First().ParameterType == typeof(PatchOperation)) args = new object[] { operation };
            else
            {
                var valueParameter = parameters.FirstOrDefault() ?? throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Add}' must declare a 'value' parameter and optionally an 'index' parameter of type 'int'");
                var indexParameter = parameters.Length > 1 ? parameters[1] : null;
                var index = (int?)null;
                if (indexParameter != null)
                {
                    var lastPathSegment = operation.Path.Segments.Last().Value;
                    if (lastPathSegment.All(c => char.IsDigit(c)))
                    {
                        if (int.TryParse(lastPathSegment, out var indexValue)) index = indexValue;
                    }
                }

                var value = (object?)null;
                if (jsonPatchOperationAttribute.ReferencedType == null)
                {
                    value = operation.Value == null ? null : JsonSerializer.Default.Deserialize(operation.Value, valueParameter.ParameterType)!;
                }
                else
                {
                    var referencedType = jsonPatchOperationAttribute.ReferencedType;
                    var keyType = referencedType.GetGenericType(typeof(IIdentifiable<>))?.GetGenericArguments()[0] ?? throw new NullReferenceException($"Failed to determine the type of key used to reference instances of type '{jsonPatchOperationAttribute.ReferencedType.Name}'");
                    var key = (operation.Value == null ? null : JsonSerializer.Default.Deserialize(operation.Value, keyType)) ?? throw new NullReferenceException($"A JSON Patch operation of type '{OperationType.Add}' must set the '{nameof(operation.Value)}' property when using references");
                    var repositoryType = typeof(IRepository<,>).MakeGenericType(referencedType, keyType);
                    var repository = (IRepository)provider.GetRequiredService(repositoryType);
                    value = Guard.Against(await repository.GetAsync(key, cancellationToken).ConfigureAwait(false)).WhenNullReference(key).Value;
                }

                args = indexParameter == null ? new object[] { value! } : new object[] { value!, index! };
            }
            method.Invoke(source, args);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Copy"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to create the delegate for</param>
    /// <param name="typeInfo">the <see cref="JsonPatchTypeInfo"/> the property to create the delegate for belongs to</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Copy"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildCopyOperationDelegateFor(PropertyInfo property, JsonPatchTypeInfo typeInfo)
    {
        return async (provider, source, operation, cancellationToken) =>
        {
            if (operation.From == null) throw new NullReferenceException($"A JSON Patch operation of type '{OperationType.Copy}' must set the '{nameof(operation.From)}' property");
            var propertySource = source is IAggregateRoot aggregate ? aggregate.State : source;
            var sourceValue = property.GetValue(propertySource);
            var valueToAdd = await typeInfo.GetValueAsync(provider, source, operation.From, cancellationToken).ConfigureAwait(false);
            if (int.TryParse(operation.Path.Segments.Last().Value, out var index))
            {
                if (property.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) throw new NotSupportedException($"Cannot index a value of type '{property.PropertyType.Name}'");
                if (sourceValue == null) throw new ArgumentOutOfRangeException($"The target property '{property.Name}' is null and cannot be indexed", nameof(index));
                if (sourceValue is IList list) list.Insert(index, valueToAdd);
                else throw new InvalidOperationException($"The target property '{property.Name}' is not a list type and it cannot be added/inserted items");
            }
            else if (sourceValue is ICollection collection)
            {
                if (valueToAdd is IEnumerable enumerable) foreach (var item in enumerable.OfType<object>().ToList()) collection.Add(item);
                else collection.Add(valueToAdd!);
            }
            else
            {
                if (valueToAdd is IEnumerable enumerable && valueToAdd.GetType() != typeof(string)) valueToAdd = enumerable.ToNonGenericList();
                property.SetValue(propertySource, valueToAdd);
            }
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Copy"/>
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create the delegate for</param>
    /// <param name="typeInfo">the <see cref="JsonPatchTypeInfo"/> the property to create the delegate for belongs to</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Copy"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildCopyOperationDelegateFor(MethodInfo method, JsonPatchTypeInfo typeInfo)
    {
        var parameters = method.GetParameters();
        if (parameters.Length < 1 || parameters.Length > 2) throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Copy}' must declare a 'values' parameter and optionally an 'index' parameter of type 'int'");
        return async (provider, source, operation, cancellationToken) =>
        {
            if (operation.From == null) throw new NullReferenceException($"A JSON Patch operation of type '{OperationType.Copy}' must set the '{nameof(operation.From)}' property");
            var sourceValue = await typeInfo.GetValueAsync(provider, source, operation.From, cancellationToken).ConfigureAwait(false);
            if (sourceValue is IEnumerable enumerable && sourceValue.GetType() != typeof(string)) sourceValue = enumerable.ToNonGenericList();
            object[] args;
            if (parameters.Length == 1 && parameters.First().ParameterType == typeof(PatchOperation)) args = new object[] { operation };
            else
            {
                var valueParameter = parameters.FirstOrDefault() ?? throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Add}' must declare a 'value' parameter and optionally an 'index' parameter of type 'int'");
                var indexParameter = parameters.Length > 1 ? parameters[1] : null;
                var index = (int?)null;
                if (indexParameter != null)
                {
                    var lastPathSegment = operation.Path.Segments.Last().Value;
                    if (lastPathSegment.All(c => char.IsDigit(c)))
                    {
                        if (int.TryParse(lastPathSegment, out var indexValue)) index = indexValue;
                    }
                }
                args = indexParameter == null ? new object[] { sourceValue! } : new object[] { sourceValue!, index! };
            }
            method.Invoke(source, args);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Move"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to create the delegate for</param>
    /// <param name="typeInfo">the <see cref="JsonPatchTypeInfo"/> the property to create the delegate for belongs to</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Move"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildMoveOperationDelegateFor(PropertyInfo property, JsonPatchTypeInfo typeInfo)
    {
        return async (provider, source, operation, cancellationToken) =>
        {
            if (operation.From == null) throw new NullReferenceException($"A JSON Patch operation of type '{OperationType.Add}' must set the '{nameof(operation.From)}' property");
            var propertySource = source is IAggregateRoot aggregate ? aggregate.State : source;
            var targetValue = property.GetValue(propertySource);
            var valueToAdd = await typeInfo.GetValueAsync(provider, source, operation.From, cancellationToken).ConfigureAwait(false);
            var sourceProperty = typeInfo.GetProperty(operation.From) ?? throw new NullReferenceException($"Failed to find a property at '{operation.Path}'");
            var sourceValues = (object?)null;

            if (int.TryParse(operation.Path.Segments.Last().Value, out var index))
            {
                if (property.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) throw new NotSupportedException($"Cannot index a value of type '{property.PropertyType.Name}'");
                if (targetValue == null) throw new ArgumentOutOfRangeException($"The target property '{property.Name}' is null and cannot be indexed", nameof(index));
                if (targetValue is IList list) list.Insert(index, valueToAdd);
                else throw new InvalidOperationException($"The target property '{property.Name}' is not a list type and it cannot be added/inserted items");
            }
            else if (targetValue is ICollection targetCollection && valueToAdd != null)
            {
                ICollection? sourceCollection = null;
                if (valueToAdd is IEnumerable enumerable)
                {
                    if (sourceProperty.Type.IsEnumerable() && sourceProperty.Type != typeof(string))
                    {
                        sourceValues = await typeInfo.GetValueAsync(provider, source, operation.From.WithoutArrayIndexer(), cancellationToken).ConfigureAwait(false);
                        sourceCollection = (ICollection)sourceValues!;
                    }
                    foreach (var item in enumerable.OfType<object>().ToList())
                    {
                        targetCollection.Add(item);
                        sourceCollection?.Remove(item);
                    }
                }
                else
                {
                    if (sourceProperty.Type.IsEnumerable() && sourceProperty.Type != typeof(string) && operation.From.IsArrayIndexer())
                    {
                        sourceValues = await typeInfo.GetValueAsync(provider, source, operation.From.WithoutArrayIndexer(), cancellationToken).ConfigureAwait(false);
                        sourceCollection = (ICollection)sourceValues!;
                        sourceCollection.Remove(valueToAdd!);
                    }
                    targetCollection.Add(valueToAdd!);
                }
            }
            else
            {
                property.SetValue(propertySource, valueToAdd);
                await typeInfo.SetValueAsync(provider, source, operation.From, null, cancellationToken).ConfigureAwait(false);
            }
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Move"/>
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create the delegate for</param>
    /// <param name="typeInfo">the <see cref="JsonPatchTypeInfo"/> the property to create the delegate for belongs to</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Move"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildMoveOperationDelegateFor(MethodInfo method, JsonPatchTypeInfo typeInfo)
    {
        var parameters = method.GetParameters();
        if (parameters.Length < 1 || parameters.Length > 2) throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Copy}' must declare a 'values' parameter and optionally an 'index' parameter of type 'int'");
        return async (provider, source, operation, cancellationToken) =>
        {
            if (operation.From == null) throw new NullReferenceException($"A JSON Patch operation of type '{OperationType.Copy}' must set the '{nameof(operation.From)}' property");
            var sourceProperty = typeInfo.GetProperty(operation.From) ?? throw new NullReferenceException($"Failed to find a property at '{operation.Path}'");
            var sourceValue = (await typeInfo.GetValueAsync(provider, source, operation.From, cancellationToken).ConfigureAwait(false))!;
            object[] args;
            if (parameters.Length == 1 && parameters.First().ParameterType == typeof(PatchOperation)) args = new object[] { operation };
            else
            {
                var valueParameter = parameters.FirstOrDefault() ?? throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Add}' must declare a 'value' parameter and optionally an 'index' parameter of type 'int'");
                var indexParameter = parameters.Length > 1 ? parameters[1] : null;
                var index = (int?)null;
                if (indexParameter != null)
                {
                    var lastPathSegment = operation.Path.Segments.Last().Value;
                    if (lastPathSegment.All(c => char.IsDigit(c)))
                    {
                        if (int.TryParse(lastPathSegment, out var indexValue)) index = indexValue;
                    }
                }
                args = indexParameter == null ? new object[] { sourceValue! } : new object[] { sourceValue!, index! };
                if (sourceProperty.Type.IsEnumerable() && sourceProperty.Type != typeof(string) && operation.From.IsArrayIndexer())
                {
                    var sourceValues = await typeInfo.GetValueAsync(provider, source, operation.From.WithoutArrayIndexer(), cancellationToken).ConfigureAwait(false);
                    if (sourceValues is ICollection collection) collection.Remove(sourceValue);
                    else throw new InvalidOperationException($"The target property at '{sourceProperty.Path}' is not a collection type and does not support removing items");
                }
                else
                {
                    await typeInfo.SetValueAsync(provider, source, operation.From, null, cancellationToken).ConfigureAwait(false);
                }
            }
            method.Invoke(source, args);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Remove"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to create the delegate for</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Remove"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildRemoveOperationDelegateFor(PropertyInfo property)
    {
        return (provider, source, operation, cancellationToken) =>
        {
            return Task.Run(() =>
            {
                var propertySource = source is IAggregateRoot aggregate ? aggregate.State : source;
                var values = property.GetValue(propertySource);
                if (operation.Path.IsArrayIndexer())
                {
                    if (values is ICollection collection) collection.Remove(collection.GetElementAt(int.Parse(operation.Path.Segments.Last().Value)));
                    else throw new InvalidOperationException($"The targeted property at path '{operation.Path}' is not a collection and does not support removing items");
                }
                else property.SetValue(propertySource, null);
            }, cancellationToken);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Remove"/>
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create the delegate for</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Remove"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildRemoveOperationDelegateFor(MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length != 1) throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Remove}' must declare exactly one 'index' parameter");
        return (provider, source, operation, cancellationToken) =>
        {
            var indexParameter = parameters.FirstOrDefault(p => typeof(int).IsAssignableFrom(p.ParameterType)) ?? throw new TargetParameterCountException($"A method used to reduce a JSON Patch operation of type '{OperationType.Remove}' must declare exactly one 'index' parameter of type 'int'");
            if (!int.TryParse(operation.Path.Segments.Last().Value, out var index)) throw new NullReferenceException($"The path of a JSON Patch operation of type '{OperationType.Remove}' must end with the index of the item to remove");
            return Task.Run(() => method.Invoke(source, new object[] { index }), cancellationToken);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Replace"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to create the delegate for</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Replace"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildReplaceOperationDelegateFor(PropertyInfo property) => BuildAddOperationDelegateFor(property);

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Replace"/>
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create the delegate for</param>
    /// <param name="jsonPatchOperationAttribute">The specified <see cref="MethodInfo"/>'s <see cref="JsonPatchOperationAttribute"/></param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Replace"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildReplaceOperationDelegateFor(MethodInfo method, JsonPatchOperationAttribute jsonPatchOperationAttribute) => BuildAddOperationDelegateFor(method, jsonPatchOperationAttribute);

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Test"/>
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to create the delegate for</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Test"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildTestOperationDelegateFor(PropertyInfo property)
    {
        return (provider, source, operation, cancellationToken) =>
        {
            return Task.Run(() =>
            {
                var propertySource = source is IAggregateRoot aggregate ? aggregate.State : source;
                var value = property.GetValue(propertySource);
                var valueType = property.PropertyType;
                if (operation.Path.IsArrayIndexer())
                {
                    var enumerableType = valueType.GetGenericType(typeof(IEnumerable<>)) ?? throw new InvalidOperationException($"The targeted property at path '{operation.Path}' is not an enumerable and does not support indexing items"); ;
                    valueType = enumerableType.GetEnumerableElementType();
                    if (value is IEnumerable enumerable) value = enumerable.GetElementAt(int.Parse(operation.Path.Segments.Last().Value));
                }
                var other = operation.Value == null ? null : JsonSerializer.Default.Deserialize(operation.Value, valueType);
                if (value?.Equals(other) != true) throw new OptimisticConcurrencyException($"The JSON Patch operation of type '{OperationType.Test}' failed for property at '{operation.Path}'.\r\nExpected value '{other}'\r\nActual value: '{value}'");
            }, cancellationToken);
        };
    }

    /// <summary>
    /// Builds a new delegate for handling a JSON Patch operation of type <see cref="OperationType.Test"/>
    /// </summary>
    /// <param name="getValueDelegate">The <see cref="Delegate"/> to build the delegate for</param>
    /// <returns>A new delegate for handling a JSON Patch operation of type <see cref="OperationType.Test"/></returns>
    public static Func<IServiceProvider, object, PatchOperation, CancellationToken, Task> BuildTestOperationDelegateFor(Delegate getValueDelegate)
    {
        return (provider, source, operation, cancellationToken) =>
        {
            return Task.Run(() =>
            {
                var propertySource = source is IAggregateRoot aggregate ? aggregate.State : source;
                var value = getValueDelegate.GetMethodInfo().Invoke(propertySource, Array.Empty<object>());
                var valueType = getValueDelegate.GetMethodInfo().ReturnType;
                if (operation.Path.IsArrayIndexer())
                {
                    var enumerableType = valueType.GetGenericType(typeof(IEnumerable<>)) ?? throw new InvalidOperationException($"The targeted property at path '{operation.Path}' is not an enumerable and does not support indexing items"); ;
                    valueType = enumerableType.GetEnumerableElementType();
                    if (value is IEnumerable enumerable) value = enumerable.GetElementAt(int.Parse(operation.Path.Segments.Last().Value));
                }
                var other = operation.Value == null ? null : JsonSerializer.Default.Deserialize(operation.Value, valueType);
                if (value?.Equals(other) != true) throw new OptimisticConcurrencyException($"The JSON Patch operation of type '{OperationType.Test}' failed for property at '{operation.Path}'.\r\nExpected value '{other}'\r\nActual value: '{value}'");
            }, cancellationToken);
        };
    }

}
