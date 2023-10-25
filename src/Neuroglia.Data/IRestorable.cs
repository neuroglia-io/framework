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
/// Defines the fundamentals of an object that can have its state restored using <see cref="ISnapshot"/>s
/// </summary>
public interface IRestorable
{

    /// <summary>
    /// Restores the object's state using the specified <see cref="ISnapshot"/>
    /// </summary>
    /// <param name="snapshot">The <see cref="ISnapshot"/> to use to restore the object's state</param>
    void Restore(ISnapshot snapshot);

}

/// <summary>
/// Defines the fundamentals of an object that can have its state restored using <see cref="ISnapshot"/>s
/// </summary>
/// <typeparam name="TSnapshot">The type of supported <see cref="ISnapshot"/>s</typeparam>
public interface IRestorable<TSnapshot>
    : IRestorable
{

    /// <summary>
    /// Restores the object's state using the specified <see cref="ISnapshot"/>
    /// </summary>
    /// <param name="snapshot">The <see cref="ISnapshot"/> to use to restore the object's state</param>
    void Restore(TSnapshot snapshot);

}