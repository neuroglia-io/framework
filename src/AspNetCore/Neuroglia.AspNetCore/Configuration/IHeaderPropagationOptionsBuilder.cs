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
namespace Neuroglia.AspNetCore.Configuration
{

    /// <summary>
    /// Represents the service used to build <see cref="HeaderPropagationOptions"/>
    /// </summary>
    public interface IHeaderPropagationOptionsBuilder
    {

        /// <summary>
        /// Propagates all headers
        /// </summary>
        void PropagateAll();

        /// <summary>
        /// Propagates the specified header
        /// </summary>
        /// <param name="name">The name of the header to propagate</param>
        /// <returns>The configured <see cref="IHeaderPropagationOptionsBuilder"/></returns>
        IHeaderPropagationOptionsBuilder Propagate(string name);

        /// <summary>
        /// Builds the <see cref="HeaderPropagationOptions"/>
        /// </summary>
        /// <returns>New <see cref="HeaderPropagationOptions"/></returns>
        HeaderPropagationOptions Build();

    }

}
