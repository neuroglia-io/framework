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

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Neuroglia.Data.Infrastructure.EventSourcing.DistributedCache.Services;

/// <summary>
/// Represents the service used to create <see cref="MemoryEventStore"/>s
/// </summary>
public class MemoryCacheEventStoreFactory
    : IFactory<MemoryEventStore>
{

    /// <inheritdoc/>
    public virtual MemoryEventStore Create() => new(new MemoryCache(Options.Create(new MemoryCacheOptions())));

    object IFactory.Create() => this.Create();

}
