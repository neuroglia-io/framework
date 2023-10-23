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

namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of an object that can create <see cref="ISnapshot"/>s of itself
/// </summary>
public interface ISnapshotable
{

    /// <summary>
    /// Creates a new <see cref="ISnapshot"/> of the object
    /// </summary>
    /// <returns>A new <see cref="ISnapshot"/> that captures the current state of the object</returns>
    ISnapshot CreateSnapshot();

}

/// <summary>
/// Defines the fundamentals of an object that can create <see cref="ISnapshot"/>s of itself
/// </summary>
/// <typeparam name="TSnapshot">The type of <see cref="ISnapshot"/>s supported by the object</typeparam>
public interface ISnapshotable<TSnapshot>
    : ISnapshotable
    where TSnapshot : class, ISnapshot
{

    /// <summary>
    /// Creates a new <see cref="ISnapshot"/> of the object
    /// </summary>
    /// <returns>A new <see cref="ISnapshot"/> that captures the current state of the object</returns>
    new TSnapshot CreateSnapshot();

}
