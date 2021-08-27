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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Neuroglia;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore
{

    /// <summary>
    /// Represents an <see cref="IModelBinderProvider"/> implementation used to provide <see cref="AbstractModelBinder"/>s instances
    /// </summary>
    public class AbstractModelBinderProvider
        : IModelBinderProvider
    {

        /// <inheritdoc/>
        public virtual IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.ModelType.IsAbstract)
                return null;
            if (!context.Metadata.ModelType.TryGetCustomAttribute(out DiscriminatorAttribute discriminatorAttribute))
                return null;
            IDictionary<string, ModelDescriptor> binders = new Dictionary<string, ModelDescriptor>();
            foreach (Type type in TypeCacheUtil.FindFilteredTypes($"neuro:{context.Metadata.ModelType.Name.ToLower()}-abstract-impl",
                t => t.IsClass && t.IsPublic && !t.IsAbstract && !t.IsNested && context.Metadata.ModelType.IsAssignableFrom(t)))
            {
                if (!type.TryGetCustomAttribute(out DiscriminatorValueAttribute discriminatorValueAttribute))
                    continue;
                ModelMetadata metadata = context.MetadataProvider.GetMetadataForType(type);
                binders.Add(discriminatorValueAttribute.Value.ToString().ToLower(), new ModelDescriptor(type, context.CreateBinder(metadata)));
            }
            return new AbstractModelBinder(context.MetadataProvider, binders, discriminatorAttribute.Property);
        }

    }

}
