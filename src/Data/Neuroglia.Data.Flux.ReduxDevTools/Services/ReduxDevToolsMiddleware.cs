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

using Microsoft.Extensions.Logging;

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents an <see cref="IMiddleware"/> used to dispatch actions to Redux dev tools plugin
    /// </summary>
    public class ReduxDevToolsMiddleware
        : IMiddleware
    {
        
        /// <summary>
        /// Initializes a new <see cref="ReduxDevToolsMiddleware"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="reduxDevToolsPlugin">The service used to interact with the Redux dev tools plugin</param>
        /// <param name="next">The next <see cref="DispatchDelegate"/> in the pipeline</param>
        public ReduxDevToolsMiddleware(ILogger<ReduxDevToolsMiddleware> logger, IReduxDevToolsPlugin reduxDevToolsPlugin, DispatchDelegate next)
        {
            this.Logger = logger;
            this.ReduxDevToolsPlugin = reduxDevToolsPlugin;
            this.Next = next;
        }
        
        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to interact with the Redux dev tools plugin
        /// </summary>
        protected IReduxDevToolsPlugin ReduxDevToolsPlugin { get; }

        /// <summary>
        /// Gets the next <see cref="DispatchDelegate"/> in the pipeline
        /// </summary>
        protected DispatchDelegate Next { get; }

        /// <inheritdoc/>
        public virtual async Task<object> InvokeAsync(IActionContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));
            var result = await this.Next.Invoke(context);
            await this.ReduxDevToolsPlugin.DispatchAsync(context.Action, result);
            return result;
        }

    }

}
