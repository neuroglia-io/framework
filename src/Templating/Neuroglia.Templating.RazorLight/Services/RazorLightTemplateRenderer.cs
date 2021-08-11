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
using RazorLight;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Templating.Services
{

    /// <summary>
    /// Represents the <see href="https://github.com/toddams/RazorLight">RazorLight</see>-based implementation of the <see cref="ITemplateRenderer"/> interface
    /// </summary>
    public class RazorLightTemplateRenderer
        : ITemplateRenderer
    {

        /// <summary>
        /// Initializes a new <see cref="RazorLightTemplateRenderer"/>
        /// </summary>
        /// <param name="razorLigthEngine">The current <see cref="IRazorLightEngine"/></param>
        public RazorLightTemplateRenderer(IRazorLightEngine razorLigthEngine)
        {
            this.RazorLightEngine = razorLigthEngine;
        }

        /// <summary>
        /// Gets the current <see cref="IRazorLightEngine"/>
        /// </summary>
        protected IRazorLightEngine RazorLightEngine { get; }

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.MD5"/> used to hash templates
        /// </summary>
        protected MD5 MD5 { get; } = MD5.Create();

        /// <inheritdoc/>
        public virtual async Task<string> RenderTemplateAsync<TModel>(string template, TModel model, IDictionary<string, object> data, CancellationToken cancellationToken = default)
        {
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(template));
            string hash = Encoding.UTF8.GetString(await this.MD5.ComputeHashAsync(stream, cancellationToken));
            return await this.RazorLightEngine.CompileRenderStringAsync(hash, template, model, data?.ToExpandoObject());
        }

        /// <inheritdoc/>
        public virtual async  Task<string> RenderTemplateAsync<TModel>(string template, TModel model, CancellationToken cancellationToken = default)
        {
            return await this.RenderTemplateAsync(template, model, new Dictionary<string, object>(), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async  Task<string> RenderTemplateAsync(string template, IDictionary<string, object> data, CancellationToken cancellationToken = default)
        {
            return await this.RenderTemplateAsync<object>(template, null, data, cancellationToken);
        }

    }

}
