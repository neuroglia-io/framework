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

using System.ComponentModel.DataAnnotations;

namespace Neuroglia.Eventing.CloudEvents.Infrastructure.Configuration;

/// <summary>
/// Represents the configuration of how to deserialize the data contained by ingested <see cref="CloudEvent"/>s of a specific type
/// </summary>
public class CloudEventIngestionConfiguration
{

    /// <summary>
    /// Gets/sets the type of <see cref="CloudEvent"/>s to deserialize
    /// </summary>
    [Required]
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets the type to deserialize the <see cref="CloudEvent.Data"/> to
    /// </summary>
    [Required]
    public virtual Type DataType { get; set; } = null!;

}
