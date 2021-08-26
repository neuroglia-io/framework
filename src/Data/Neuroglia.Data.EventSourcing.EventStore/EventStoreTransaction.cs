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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.Data.EventSourcing.EventStore
{

    /// <summary>
    /// Represents the <see href="https://www.eventstore.com/">Event Store</see> implementation of an <see cref="ITransaction"/>
    /// </summary>
    public class EventStoreTransaction
        : ITransaction
    {

        /// <summary>
        /// Represents the event fired whenever the <see cref="EventStoreTransaction"/> has been disposed of
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes a new <see cref="EventStoreTransaction"/>
        /// </summary>
        /// <param name="underlyingTransaction">The underlying <see cref="global::EventStore.ClientAPI.EventStoreTransaction"/></param>
        public EventStoreTransaction(global::EventStore.ClientAPI.EventStoreTransaction underlyingTransaction)
        {
            this.UnderlyingTransaction = underlyingTransaction;
        }

        /// <summary>
        /// Gets the underlying <see cref="global::EventStore.ClientAPI.EventStoreTransaction"/>
        /// </summary>
        public global::EventStore.ClientAPI.EventStoreTransaction UnderlyingTransaction { get; }

        /// <inheritdoc/>
        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await this.UnderlyingTransaction.CommitAsync();
        }

        /// <inheritdoc/>
        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => this.UnderlyingTransaction.Rollback(), cancellationToken);
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="EventStoreTransaction"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="EventStoreTransaction"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.UnderlyingTransaction.Dispose();
                    this.Disposed?.Invoke(this, new EventArgs());
                }
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public virtual async ValueTask DisposeAsync()
        {
            await Task.Run(() => this.Dispose());
        }

    }

}
