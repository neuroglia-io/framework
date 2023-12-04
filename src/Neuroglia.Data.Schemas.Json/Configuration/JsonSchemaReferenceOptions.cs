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

namespace Neuroglia.Data.Schemas.Json.Configuration;

/// <summary>
/// Represents the options used to configure references to external <see cref="JsonSchema"/>s to load
/// </summary>
public class JsonSchemaReferenceOptions
{

    /// <summary>
    /// Gets/sets the <see cref="System.Uri"/> used to reference the <see cref="JsonSchema"/> to load
    /// </summary>
    [Required]
    public virtual Uri Uri { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing functions, if any, used to mutate the external <see cref="JsonSchema"/> before registration
    /// </summary>
    public virtual List<Func<IServiceProvider, JsonSchema, Task<JsonSchema>>>? Mutators { get; set; }

}
