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

using Neuroglia.Data.Infrastructure.ResourceOriented.Services;

namespace Neuroglia.Data.Infrastructure.ResourceOriented.Configuration;

/// <summary>
/// Represents the options used to configure a <see cref="ResourceController{TResource}"/>
/// </summary>
public class ResourceControllerOptions<TResource>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Gets/sets the namespace to watch for resource events, if any
    /// </summary>
    public virtual string? ResourceNamespace { get; set; }

    /// <summary>
    /// Gets/sets a list containing the label-based selectors of controlled <see cref="IResource"/>s
    /// </summary>
    public virtual List<LabelSelector>? LabelSelectors { get; set; }

    /// <summary>
    /// Gets/sets the options used to configure the controller's reconciliation loop
    /// </summary>
    public virtual ResourceControllerReconciliationOptions Reconciliation { get; set; } = new();

}
