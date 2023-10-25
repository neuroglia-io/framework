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

using Microsoft.CSharp.RuntimeBinder;

namespace Neuroglia.Data;

/// <summary>
/// Represents an <see cref="IRestorable"/> implementation of the <see cref="IAggregateRoot"/> interface
/// </summary>
/// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/></typeparam>
/// <typeparam name="TState">The type of the <see cref="IAggregateRoot"/>'s state</typeparam>
public abstract class RestorableAggregateRoot<TKey, TState>
    : AggregateRoot<TKey, TState>, IRestorable
    where TKey : IEquatable<TKey>
    where TState : class, IAggregateState<TKey>, new()
{

    bool _restoring;

    void IRestorable.Restore(ISnapshot snapshot)
    {
        if (this._restoring) throw new RuntimeBinderException($"Failed to bind the specified snapshot of type '{snapshot.GetType().Name}' to a recoverable method");
        this._restoring = true;
        try { ((dynamic)this).Restore((dynamic)snapshot); }
        catch (RuntimeBinderException) { throw new RuntimeBinderException($"Failed to bind the specified snapshot of type '{snapshot.GetType().Name}' to a recoverable method"); }
        this._restoring = false;
    }

}