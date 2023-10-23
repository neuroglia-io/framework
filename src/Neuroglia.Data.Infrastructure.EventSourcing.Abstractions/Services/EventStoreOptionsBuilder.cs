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

using Neuroglia.Data.Infrastructure.EventSourcing.Configuration;
using Neuroglia.Serialization;

namespace Neuroglia.Data.Infrastructure.EventSourcing.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventStoreOptionsBuilder"/> interface
/// </summary>
public class EventStoreOptionsBuilder
    : IEventStoreOptionsBuilder
{

    /// <summary>
    /// Gets the <see cref="EventStoreOptions"/> to configure
    /// </summary>
    protected EventStoreOptions Options { get; } = new();

    /// <inheritdoc/>
    public virtual IEventStoreOptionsBuilder UseSerializer<TSerializer>()
        where TSerializer : class, ISerializer
    {
        this.Options.SerializerType = typeof(TSerializer);
        return this;
    }

    /// <inheritdoc/>
    public virtual EventStoreOptions Build() => this.Options;

}
