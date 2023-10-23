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

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Represents the snapshot envelope of an <see cref="IAggregateRoot"/>
/// </summary>
public class Snapshot
    : ISnapshot
{

    /// <summary>
    /// Initializes a new <see cref="Snapshot"/>
    /// </summary>
    protected Snapshot() { }

    /// <summary>
    /// Initializes a new <see cref="Snapshot"/>
    /// </summary>
    /// <param name="state">The state to snapshot</param>
    /// <param name="stateVersion">The version of the state to snapshot</param>
    /// <param name="metadata">The metadata, if any, of the <see cref="Snapshot{TState}"/> to create</param>
    public Snapshot(object state, ulong stateVersion, IDictionary<string, object>? metadata = null)
    {
        this.State = state ?? throw new ArgumentNullException(nameof(state));
        this.StateVersion = stateVersion;
        this.Metadata = metadata;
    }

    /// <inheritdoc/>
    public virtual object State { get; protected set; } = null!;

    object ISnapshot.State => this.State;

    /// <inheritdoc/>
    public virtual ulong StateVersion { get; protected set; }

    /// <inheritdoc/>
    public virtual IDictionary<string, object>? Metadata { get; protected set; }

}


/// <summary>
/// Represents the snapshot envelope of an <see cref="IAggregateRoot"/>
/// </summary>
/// <typeparam name="TState">The type of the snapshot <see cref="IAggregateRoot"/></typeparam>
public class Snapshot<TState>
    : Snapshot, ISnapshot<TState>
    where TState : class
{

    /// <summary>
    /// Initializes a new <see cref="Snapshot{TKey}"/>
    /// </summary>
    protected Snapshot() { }

    /// <summary>
    /// Initializes a new <see cref="Snapshot{TKey}"/>
    /// </summary>
    /// <param name="state">The state to snapshot</param>
    /// <param name="stateVersion">The version of the state to snapshot</param>
    /// <param name="metadata">The metadata, if any, of the <see cref="Snapshot{TState}"/> to create</param>
    public Snapshot(TState state, ulong stateVersion, IDictionary<string, object>? metadata = null)
    {
        this.State = state ?? throw new ArgumentNullException(nameof(state));
        this.StateVersion = stateVersion;
        this.Metadata = metadata;
    }

    /// <inheritdoc/>
    public virtual new TState State
    {
        get => (TState)base.State;
        set => base.State = value;
    }

}
