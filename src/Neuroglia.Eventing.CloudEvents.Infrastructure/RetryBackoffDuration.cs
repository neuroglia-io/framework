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

namespace Neuroglia.Eventing.CloudEvents.Infrastructure;

/// <summary>
/// Represents an object used to configure a retry backoff duration
/// </summary>
public record RetryBackoffDuration
{

    /// <summary>
    /// Gets/sets the duration's type
    /// </summary>
    public virtual RetryBackoffDurationType Type { get; set; } = RetryBackoffDurationType.Exponential;

    /// <summary>
    /// Gets/sets the period of time to wait between retry attempts 
    /// </summary>
    public virtual TimeSpan Period { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets/sets a value representing the power to which the specified period of time is to be raised to obtain the time to wait between each retry attempts
    /// </summary>
    public virtual double? Exponent { get; set; } = 2;

    /// <summary>
    /// Gets/sets a boolean indicating whether or not to use jitter
    /// </summary>
    public virtual bool Jitter { get; set; } = true;

    /// <summary>
    /// Gets/sets the minimum jitter to apply, in milliseconds, if any
    /// </summary>
    public virtual int? MinJitter { get; set; } = 1;

    /// <summary>
    /// Gets/sets the maximum jitter to apply, in milliseconds, if any
    /// </summary>
    public virtual int? MaxJitter { get; set; } = 1000;

}
