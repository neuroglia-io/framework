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

namespace Neuroglia.Eventing.CloudEvents.Infrastructure.Configuration;

/// <summary>
/// Represents an object used to configure a retry policy
/// </summary>
public class RetryPolicyOptions
{

    /// <summary>
    /// Gets/sets an object used to configure the backoff duration between retry attempts
    /// </summary>
    public virtual RetryBackoffDuration BackoffDuration { get; set; } = new();

    /// <summary>
    /// Gets/sets the maximum retry attempts to perform. If not set, it will retry forever
    /// </summary>
    public virtual int? MaxAttempts { get; set; } = 3;

}
