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

namespace Neuroglia;

/// <summary>
/// Exposes default Neuroglia error types
/// </summary>
public static class ErrorTypes
{

    static readonly Uri BaseUri = new("https://neuroglia.io/docs/errors/");

    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing an invalid request
    /// </summary>
    public static readonly Uri Invalid = new(BaseUri, "invalid");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a conflict
    /// </summary>
    public static readonly Uri Conflict = new(BaseUri, "conflict");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a resource
    /// </summary>
    public static readonly Uri NotFound = new(BaseUri, "not-found");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to patch a resource
    /// </summary>
    public static readonly Uri NotModified = new(BaseUri, "not-modified");

}