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
namespace Neuroglia.Data
{
    /// <summary>
    /// Represents an implementation of the <see cref="IPluralizer"/> interface based on a <see cref="Pluralize.NET.Core.Pluralizer"/> service
    /// </summary>
    public class Pluralizer
        : IPluralizer
    {

        /// <summary>
        /// Initializes a new <see cref="Pluralizer"/>
        /// </summary>
        public Pluralizer()
        {
            this.PluralizationService = new Pluralize.NET.Core.Pluralizer();
        }

        /// <summary>
        /// Gets the underlying <see cref="Pluralize.NET.Core.Pluralizer"/>
        /// </summary>
        protected Pluralize.NET.Core.Pluralizer PluralizationService { get; }

        /// <inheritdoc/>
        public virtual string Pluralize(string word)
        {
            return this.PluralizationService.Pluralize(word);
        }

        /// <inheritdoc/>
        public virtual string Singularize(string word)
        {
            return this.PluralizationService.Singularize(word);
        }

    }


}
