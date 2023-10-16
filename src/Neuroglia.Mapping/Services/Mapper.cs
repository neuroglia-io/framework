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

using System.Collections;
using System.Diagnostics;

namespace Neuroglia.Mapping;

/// <summary>
/// Represents the default AutoMapper implementation of the <see cref="IMapper"/> interface
/// </summary>
public class Mapper
    : IMapper
{

    /// <summary>
    /// Gets the <see cref="Mapper"/>'s <see cref="ActivitySource"/> name
    /// </summary>
    public const string ActivitySourceName = "Neuroglia.Mapping.Diagnostics.ActivitySource";

    private static readonly ActivitySource _ActivitySource = new(ActivitySourceName);

    /// <summary>
    /// Initializes a new <see cref="Mapper"/>
    /// </summary>
    /// <param name="innerMapper">The underlying <see cref="AutoMapper.IMapper"/></param>
    public Mapper(AutoMapper.IMapper innerMapper)
    {
        this.InnerMapper = innerMapper;
    }

    /// <summary>
    /// Gets the underlying <see cref="AutoMapper.IMapper"/>
    /// </summary>
    protected AutoMapper.IMapper InnerMapper { get; }

    /// <inheritdoc/>
    public virtual TDestination Map<TDestination>(object source)
    {
        using var activity = _ActivitySource.StartActivity($"{(source is null ? "null" : source.GetType().Name)} => {typeof(TDestination).Name}");
        activity?.AddTag("mapping.source_type", source?.GetType().Name);
        activity?.AddTag("mapping.destination_type", typeof(TDestination).Name);
        var result = this.InnerMapper.Map<TDestination>(source);
        return result;
    }

    /// <inheritdoc/>
    public virtual TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> configuration)
    {
        using var activity = _ActivitySource.StartActivity($"{(source is null ? "null" : source.GetType().Name)} => {typeof(TDestination).Name}");
        activity?.AddTag("mapping.source_type", source?.GetType().Name);
        activity?.AddTag("mapping.destination_type", typeof(TDestination).Name);
        var result = this.InnerMapper.Map<TDestination>(source, this.BuildMappingOperationOptions(configuration));
        return result;
    }

    /// <inheritdoc/>
    public virtual TDestination Map<TSource, TDestination>(TSource source)
    {
        using var activity = _ActivitySource.StartActivity($"{(source is null ? "null" : source.GetType().Name)} => {typeof(TDestination).Name}");
        activity?.AddTag("mapping.source_type", source?.GetType().Name);
        activity?.AddTag("mapping.destination_type", typeof(TDestination).Name);
        var result = this.InnerMapper.Map<TSource, TDestination>(source);
        return result;
    }

    /// <inheritdoc/>
    public virtual TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> configuration)
    {
        using var activity = _ActivitySource.StartActivity($"{typeof(TSource).Name} => {typeof(TDestination).Name}");
        activity?.AddTag("mapping.source_type", typeof(TSource).Name);
        activity?.AddTag("mapping.destination_type", typeof(TDestination).Name);
        TDestination result = this.InnerMapper.Map(source, this.BuildMappingOperationOptions(configuration));
        return result;
    }

    /// <inheritdoc/>
    public virtual TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        using var activity = _ActivitySource.StartActivity($"{typeof(TSource).Name} => {typeof(TDestination).Name}");
        activity?.AddTag("mapping.source_type", typeof(TSource).Name);
        activity?.AddTag("mapping.destination_type", typeof(TDestination).Name);
        var result = this.InnerMapper.Map(source, destination);
        return result;
    }

    /// <inheritdoc/>
    public virtual TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> configuration)
    {
        using var activity = _ActivitySource.StartActivity($"{typeof(TSource).Name} => {typeof(TDestination).Name}");
        activity?.AddTag("mapping.source_type", typeof(TSource).Name);
        activity?.AddTag("mapping.destination_type", typeof(TDestination).Name);
        var result = this.InnerMapper.Map(source, destination, this.BuildMappingOperationOptions(configuration));
        return result;
    }

    /// <inheritdoc/>
    public virtual object Map(object source, Type sourceType, Type destinationType)
    {
        using var activity = _ActivitySource.StartActivity($"{sourceType.Name} => {destinationType.Name}");
        activity?.AddTag("mapping.source_type", sourceType.Name);
        activity?.AddTag("mapping.destination_type", destinationType.Name);
        var result = this.InnerMapper.Map(source, sourceType, destinationType);
        return result;
    }

    /// <inheritdoc/>
    public virtual object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> configuration)
    {
        using var activity = _ActivitySource.StartActivity($"{sourceType.Name} => {destinationType.Name}");
        activity?.AddTag("mapping.source_type", sourceType.Name);
        activity?.AddTag("mapping.destination_type", destinationType.Name);
        var result = this.InnerMapper.Map(source, sourceType, destinationType, this.BuildMappingOperationOptions(configuration));
        return result;
    }

    /// <inheritdoc/>
    public virtual IEnumerable<TDestination> ProjectTo<TDestination>(IEnumerable source) => source.OfType<object>().Select(this.InnerMapper.Map<TDestination>).ToList();

    /// <inheritdoc/>
    public virtual IEnumerable<TDestination> ProjectTo<TDestination>(IEnumerable source, Action<IMappingOperationOptions> configuration) => source.OfType<object>().Select(e => this.InnerMapper.Map<TDestination>(e, this.BuildMappingOperationOptions(configuration))).ToList();

    /// <inheritdoc/>
    public virtual IEnumerable<TDestination> ProjectTo<TSource, TDestination>(IEnumerable<TSource> source) => source.Select(this.InnerMapper.Map<TSource, TDestination>).ToList();

    /// <inheritdoc/>
    public virtual IEnumerable<TDestination> ProjectTo<TSource, TDestination>(IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> configuration) => source.Select(e => this.InnerMapper.Map(e, this.BuildMappingOperationOptions(configuration))).ToList();

    /// <inheritdoc/>
    public virtual IEnumerable<object> ProjectTo(IEnumerable source, Type sourceType, Type destinationType) => source.OfType<object>().Select(e => this.InnerMapper.Map(e, sourceType, destinationType)).ToList();

    /// <inheritdoc/>
    public virtual IEnumerable<object> ProjectTo(IEnumerable source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> configuration) => source.OfType<object>().Select(e => this.InnerMapper.Map(e, sourceType, destinationType)).ToList();

    /// <summary>
    /// Gets the <see cref="AutoMapper.IMappingOperationOptions"/> from the specified <see cref="IMappingOperationOptions"/>
    /// </summary>
    /// <param name="configuration">The <see cref="IMappingOperationOptions"/> to build the <see cref="AutoMapper.IMappingOperationOptions"/> for</param>
    /// <returns>A new <see cref="AutoMapper.IMappingOperationOptions"/></returns>
    protected virtual Action<AutoMapper.IMappingOperationOptions> BuildMappingOperationOptions(Action<IMappingOperationOptions> configuration)
    {
        var options = new MappingOperationOptions();
        configuration(options);
        return opts =>
        {
            foreach (var kvp in options.Items)
            {
                opts.Items.Add(kvp.Key, kvp.Value);
            }
        };
    }

    /// <summary>
    /// Gets the <see cref="AutoMapper.IMappingOperationOptions{TSource, TDestination}"/> from the specified <see cref="IMappingOperationOptions{TSource, TDestination}"/>
    /// </summary>
    /// <typeparam name="TSource">The type to map from</typeparam>
    /// <typeparam name="TDestination">The type to map to</typeparam>
    /// <param name="configuration">The <see cref="IMappingOperationOptions{TSource, TDestination}"/> to build the <see cref="AutoMapper.IMappingOperationOptions{TSource, TDestination}"/> for</param>
    /// <returns>A new <see cref="AutoMapper.IMappingOperationOptions{TSource, TDestination}"/></returns>
    protected virtual Action<AutoMapper.IMappingOperationOptions<TSource, TDestination>> BuildMappingOperationOptions<TSource, TDestination>(Action<IMappingOperationOptions<TSource, TDestination>> configuration)
    {
        var options = new MappingOperationOptions<TSource, TDestination>();
        configuration(options);
        return opts =>
        {
            foreach (var kvp in options.Items)
            {
                opts.Items.Add(kvp.Key, kvp.Value);
            }
        };
    }

}
