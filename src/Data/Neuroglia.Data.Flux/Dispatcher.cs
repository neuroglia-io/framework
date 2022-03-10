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

namespace Neuroglia.Data.Flux
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IDispatcher"/> interface
    /// </summary>
    public class Dispatcher
        : IDispatcher
    {

        /// <summary>
        /// Gets the <see cref="Subject"/> used to stream actions
        /// </summary>
        protected Subject<object> Stream { get; } = new();

        /// <inheritdoc/>
        public void Dispatch(object action)
        {
            if(action == null)
                throw new ArgumentNullException(nameof(action));  
            this.Stream.OnNext(action);
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IObserver<object> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            return this.Stream.Subscribe(observer);
        }

    }

}
