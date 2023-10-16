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

/*
 * Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
using System;

namespace Neuroglia.Data.Infrastructure.EventSourcing.EventStore
{

    /// <summary>
    /// Defines the fundamentals of an <see href="https://eventstore.com/">Event Store</see> subscription
    /// </summary>
    public interface IEventStoreSubscription
        : IDisposable
    {

        /// <summary>
        /// Represents the event fired whenever the <see cref="IEventStoreSubscription"/> has been disposed of
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets the <see cref="IEventStoreSubscription"/>'s id
        /// </summary>
        string Id { get; }

    }

}
