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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Templating.Services
{

    /// <summary>
    /// Defines the fundamentals of a service used to render templates
    /// </summary>
    public interface ITemplateRenderer
    {

        /// <summary>
        /// Renders the specified template
        /// </summary>
        /// <typeparam name="TModel">The type of the template model</typeparam>
        /// <param name="template">The template to render</param>
        /// <param name="model">The template's model</param>
        /// <param name="data">An <see cref="IDictionary{TKey, TValue}"/> containing the template's data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The rendered template</returns>
        Task<string> RenderTemplateAsync<TModel>(string template, TModel model, IDictionary<string, object> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renders the specified template
        /// </summary>
        /// <typeparam name="TModel">The type of the template model</typeparam>
        /// <param name="template">The template to render</param>
        /// <param name="model">The template's model</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The rendered template</returns>
        Task<string> RenderTemplateAsync<TModel>(string template, TModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renders the specified template
        /// </summary>
        /// <param name="template">The template to render</param>
        /// <param name="data">An <see cref="IDictionary{TKey, TValue}"/> containing the template's data</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The rendered template</returns>
        Task<string> RenderTemplateAsync(string template, IDictionary<string, object> data, CancellationToken cancellationToken = default);

    }

}
