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

using Neuroglia.Data.Flux.Components;

namespace Neuroglia.Data.Flux.Configuration
{
    /// <summary>
    /// Represents the object used to configure a <see cref="FluxStore"/> component
    /// </summary>
    public class FluxStoreOptions
    {

		/// <summary>
		/// Gets/sets the name to display in the Redux Dev Tools window
		/// </summary>
		public string Name { get; set; } = "Flux";

		/// <summary>
		/// Gets/sets a <see cref="TimeSpan"/> indicating how often the Redux Dev Tools actions should be updated. Defaults to 50.
		/// </summary>
		public TimeSpan Latency { get; set; } = TimeSpan.FromMilliseconds(50);

		/// <summary>
		/// Gets/sets a value used to configure the Redux Dev Tools max age setting. Defaults to 50.
		/// </summary>
		public ushort MaxAge { get; set; } = 50;

		/// <inheritdoc/>
		public override string ToString()
        {
			return $@"name:""{this.Name}"", maxAge:{this.MaxAge}, latency:{this.Latency.TotalMilliseconds}";
		}

	}

}
