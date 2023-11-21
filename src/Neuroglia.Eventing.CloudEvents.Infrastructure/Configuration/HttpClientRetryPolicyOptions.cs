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
/// Represents an object used to configure the retry policy for an http client
/// </summary>
public class HttpClientRetryPolicyOptions
    : RetryPolicyOptions
{

    /// <summary>
    /// Gets/sets a list containing the http status codes the retry policy applies to. If not set, the policy will apply to all non-success (200-300) status codes
    /// </summary>
    public virtual List<int>? StatusCodes { get; set; }

    /// <summary>
    /// Gets/sets an object that configures the client's circuit breaker, if any
    /// </summary>
    public virtual CircuitBreakerPolicyOptions? CircuitBreaker { get; set; }

}
