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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore
{
    /// <summary>
    /// Represents an <see cref="IModelBinder"/> implementation used to bind abstract classes
    /// </summary>
    public class AbstractModelBinder
        : IModelBinder
    {

        /// <summary>
        /// Initializes a new <see cref="AbstractModelBinder"/>
        /// </summary>
        /// <param name="metadataProvider">The service used to provide <see cref="ModelMetadata"/></param>
        /// <param name="descriptors">An <see cref="IDictionary{TKey, TValue}"/> containing all resolved <see cref="ModelDescriptor"/>s</param>
        /// <param name="discriminatorPropertyName">The name of the discriminator property to use</param>
        public AbstractModelBinder(IModelMetadataProvider metadataProvider, IDictionary<string, ModelDescriptor> descriptors, string discriminatorPropertyName)
        {
            this.MetadataProvider = metadataProvider;
            this.Descriptors = descriptors;
            this.DiscriminatorPropertyName = discriminatorPropertyName;
        }

        /// <summary>
        /// Gets the service used to provide <see cref="ModelMetadata"/>
        /// </summary>
        protected IModelMetadataProvider MetadataProvider { get; }

        /// <summary>
        /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing all resolved <see cref="ModelDescriptor"/>s
        /// </summary>
        protected IDictionary<string, ModelDescriptor> Descriptors { get; }

        /// <summary>
        /// Gets the name of the discriminator property to use
        /// </summary>
        protected string DiscriminatorPropertyName { get; }

        /// <inheritdoc/>
        public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string discriminatorValueModelName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, this.DiscriminatorPropertyName);
            ValueProviderResult discriminatorValueResult = bindingContext.ValueProvider.GetValue(discriminatorValueModelName);
            if (discriminatorValueResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            if (!this.Descriptors.TryGetValue(discriminatorValueResult.FirstValue.ToLower(), out ModelDescriptor descriptor))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            ModelMetadata metadata = this.MetadataProvider.GetMetadataForType(descriptor.Type);
            ModelBindingResult result;
            ModelMetadata innerMetadata;
            using (bindingContext.EnterNestedScope(metadata, bindingContext.FieldName, bindingContext.ModelName, model: null))
            {
                await descriptor.Binder.BindModelAsync(bindingContext);
                result = bindingContext.Result;
                innerMetadata = bindingContext.ModelMetadata;
            }
            bindingContext.Result = result;
            bindingContext.ValidationState.Add(result.Model, new ValidationStateEntry()
            {
                Metadata = innerMetadata
            });
            return;
        }

    }

}
