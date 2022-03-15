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
using Microsoft.JSInterop;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IReduxDevToolsPlugin"/> interface
    /// </summary>
    public class ReduxDevToolsPlugin
        : IReduxDevToolsPlugin
    {

        /// <summary>
        /// Gets the name of the method invoke whenever the <see cref="ReduxDevToolsPlugin"/> has been detected
        /// </summary>
        public const string JSPrefix = "__BladuxDevTools__";
        /// <summary>
        /// Gets the name of the JavaScript method used to initialize the Redux dev tools
        /// </summary>
        public const string JSInitializeMethodName = "initialize";
        /// <summary>
        /// Gets the name of the JavaScript method used to dispatch actions to Redux
        /// </summary>
        public const string JSDispatchMethodName = "dispatch";
        /// <summary>
        /// Gets the name of the method invoked as a Redux dev tools callback
        /// </summary>
        public const string JSCallbackMethodName = "callback";
        /// <summary>
        /// Gets the name of the method invoke whenever the <see cref="ReduxDevToolsPlugin"/> has been detected
        /// </summary>
        public const string JSOnDetectedMethodName = "onDetected";

        private bool _Disposed;

        /// <summary>
        /// Initializes a new <see cref="ReduxDevToolsPlugin"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="jsRuntime">The service used to interact with the JavaScript runtime</param>
        /// <param name="jsonSerializer">The service used to serialize/deserialize objects to/from JSON</param>
        /// <param name="store">The current <see cref="IStore"/></param>
        public ReduxDevToolsPlugin(ILogger<ReduxDevToolsPlugin> logger, IJSRuntime jsRuntime, IJsonSerializer jsonSerializer, IStore store)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.JSRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            this.JsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            this.Store = store ?? throw new ArgumentNullException(nameof(store));
            this.Reference = DotNetObjectReference.Create(this);
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to interact with the JavaScript runtime
        /// </summary>
        protected IJSRuntime JSRuntime { get; }

        /// <summary>
        /// Gets the service used to serialize/deserialize objects to/from JSON
        /// </summary>
        protected IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// Gets the current <see cref="IStore"/>
        /// </summary>
        protected IStore Store { get; }

        /// <summary>
        /// Gets the <see cref="ReduxDevToolsPlugin"/>'s JS reference
        /// </summary>
        protected DotNetObjectReference<ReduxDevToolsPlugin> Reference { get; }

        /// <inheritdoc/>
        public bool IsEnabled { get; protected set; }

        /// <summary>
        /// Gets a boolean indicating whether or not the <see cref="ReduxDevToolsPlugin"/> is initializing
        /// </summary>
        protected bool IsInitializing { get; set; }

        /// <summary>
        /// Gets a boolean indicating whether or not the <see cref="ReduxDevToolsPlugin"/> has been initialized
        /// </summary>
        protected bool Initialized { get; set; }

        /// <inheritdoc/>
        public virtual async Task InitializeStateAsync()
        {
            this.IsInitializing = true;
            try
            {
                await this.InvokeJSRuntimeMethodAsync<object>(JSInitializeMethodName, this.Reference, this.Store.State);
            }
            finally
            {
                this.IsInitializing = false;
                this.Initialized = true;
            }
        }

        /// <inheritdoc/>
        public virtual async Task DispatchAsync(object action, object state)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (!this.Initialized)
                await this.InitializeStateAsync();
            await this.InvokeJSRuntimeMethodAsync<object>(JSDispatchMethodName, action, state);
        }

        /// <summary>
        /// Invokes the specified JavaScript function
        /// </summary>
        /// <typeparam name="TResult">The expected type of result</typeparam>
        /// <param name="methodName">The name of the method to invoke</param>
        /// <param name="args">An array that contains the arguments to invoke the JavaScript function with</param>
        /// <returns>The result of the JavaScript function invocation</returns>
        protected virtual async Task<TResult> InvokeJSRuntimeMethodAsync<TResult>(string methodName, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));
            if (!this.IsEnabled && !this.IsInitializing)
                return default!;
            return await this.JSRuntime.InvokeAsync<TResult>($"{JSPrefix}.{methodName}", args);
        }

        /// <summary>
        /// Represents the method invoked as callback by the Redux dev tools 
        /// </summary>
        /// <param name="json">The JSON representation of the message sent by the Redux dev tools callback</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
		[JSInvokable(JSCallbackMethodName)]
        public async Task OnCallbackAsync(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;
            var message = await this.JsonSerializer.DeserializeAsync<ReduxMessage>(json);
            switch (message?.Payload?.Type)
            {
                case JSOnDetectedMethodName:
                    this.IsEnabled = true;
                    break;
                case ReduxMessagePayloadType.Commit:
                    await this.InitializeStateAsync();
                    break;
                case ReduxMessagePayloadType.JumpToState:
                case ReduxMessagePayloadType.JumpToAction:
                    //todo: react to Redux
                    break;
                default:
                    if (!string.IsNullOrWhiteSpace(message?.Payload?.Type))
                        this.Logger.LogWarning("The specified Redux message payload type '{payloadType}' is not supported", message.Payload.Type);
                    break;
            }
        }

        /// <summary>
        /// Disposes of the <see cref="ReduxDevToolsPlugin"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="ReduxDevToolsPlugin"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                    this.Reference?.Dispose();
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
