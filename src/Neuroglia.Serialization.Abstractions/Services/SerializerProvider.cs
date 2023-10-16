// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.DependencyInjection;

namespace Neuroglia.Serialization;

/// <summary>
/// Represents the default implementation of the <see cref="ISerializerProvider"/> interface
/// </summary>
public class SerializerProvider
    : ISerializerProvider
{

    /// <summary>
    /// Initializes a new <see cref="SerializerProvider"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    public SerializerProvider(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all configured <see cref="ISerializer"/>s
    /// </summary>
    protected IEnumerable<ISerializer> Serializers => this.ServiceProvider.GetServices<ISerializer>();

    /// <inheritdoc/>
    public IEnumerable<ISerializer> GetSerializers() => this.Serializers;

    /// <inheritdoc/>
    public IEnumerable<ISerializer> GetSerializersFor(string mediaTypeName) => this.Serializers.Where(s => s.Supports(mediaTypeName));

}