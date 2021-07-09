/*
 * Copyright © 2021 Neuroglia SPRL. All rights reserved.
 * <p>
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * <p>
 * http://www.apache.org/licenses/LICENSE-2.0
 * <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Neuroglia.Mapping
{
    /// <summary>
    /// Represents the default AutoMapper implementation of the <see cref="IMapper"/> interface
    /// </summary>
    public class Mapper
        : IMapper
    {

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
            TDestination result = this.InnerMapper.Map<TDestination>(source);
            return result;
        }

        /// <inheritdoc/>
        public virtual TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> configuration)
        {
            TDestination result = this.InnerMapper.Map<TDestination>(source, this.BuildMappingOperationOptions(configuration));
            return result;
        }

        /// <inheritdoc/>
        public virtual TDestination Map<TSource, TDestination>(TSource source)
        {
            TDestination result = this.InnerMapper.Map<TSource, TDestination>(source);
            return result;
        }

        /// <inheritdoc/>
        public virtual TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> configuration)
        {
            TDestination result = this.InnerMapper.Map(source, this.BuildMappingOperationOptions(configuration));
            return result;
        }

        /// <inheritdoc/>
        public virtual TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            TDestination result = this.InnerMapper.Map(source, destination);
            return result;
        }

        /// <inheritdoc/>
        public virtual TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> configuration)
        {
            TDestination result = this.InnerMapper.Map(source, destination, this.BuildMappingOperationOptions(configuration));
            return result;
        }

        /// <inheritdoc/>
        public virtual object Map(object source, Type sourceType, Type destinationType)
        {
            object result = this.InnerMapper.Map(source, sourceType, destinationType);
            return result;
        }

        /// <inheritdoc/>
        public virtual object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> configuration)
        {
            object result = this.InnerMapper.Map(source, sourceType, destinationType, this.BuildMappingOperationOptions(configuration));
            return result;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<TDestination> ProjectTo<TDestination>(IEnumerable source)
        {
            IEnumerable<TDestination> result = source.OfType<object>().Select(e => this.InnerMapper.Map<TDestination>(e)).ToList();
            return result;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<TDestination> ProjectTo<TDestination>(IEnumerable source, Action<IMappingOperationOptions> configuration)
        {
            IEnumerable<TDestination> result = source.OfType<object>().Select(e => this.InnerMapper.Map<TDestination>(e, this.BuildMappingOperationOptions(configuration))).ToList();
            return result;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<TDestination> ProjectTo<TSource, TDestination>(IEnumerable<TSource> source)
        {
            IEnumerable<TDestination> result = source.Select(e => this.InnerMapper.Map<TSource, TDestination>(e)).ToList();
            return result;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<TDestination> ProjectTo<TSource, TDestination>(IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> configuration)
        {
            IEnumerable<TDestination> result = source.Select(e => this.InnerMapper.Map(e, this.BuildMappingOperationOptions(configuration))).ToList();
            return result;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<object> ProjectTo(IEnumerable source, Type sourceType, Type destinationType)
        {
            IEnumerable<object> result = source.OfType<object>().Select(e => this.InnerMapper.Map(e, sourceType, destinationType)).ToList();
            return result;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<object> ProjectTo(IEnumerable source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> configuration)
        {
            IEnumerable<object> result = source.OfType<object>().Select(e => this.InnerMapper.Map(e, sourceType, destinationType)).ToList();
            return result;
        }

        /// <summary>
        /// Gets the <see cref="AutoMapper.IMappingOperationOptions"/> from the specified <see cref="IMappingOperationOptions"/>
        /// </summary>
        /// <param name="configuration">The <see cref="IMappingOperationOptions"/> to build the <see cref="AutoMapper.IMappingOperationOptions"/> for</param>
        /// <returns>A new <see cref="AutoMapper.IMappingOperationOptions"/></returns>
        protected virtual Action<AutoMapper.IMappingOperationOptions> BuildMappingOperationOptions(Action<IMappingOperationOptions> configuration)
        {
            MappingOperationOptions options = new MappingOperationOptions();
            configuration(options);
            return opts =>
            {
                foreach (KeyValuePair<string, string> kvp in options.Items)
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
            MappingOperationOptions<TSource, TDestination> options = new MappingOperationOptions<TSource, TDestination>();
            configuration(options);
            return opts =>
            {
                foreach (KeyValuePair<string, string> kvp in options.Items)
                {
                    opts.Items.Add(kvp.Key, kvp.Value);
                }
            };
        }

    }


}
