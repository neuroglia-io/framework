﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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

namespace Neuroglia.Eventing.CloudEvents;

/// <summary>
/// Exposes <see cref="ICloudEvent"/> content types
/// </summary>
public static class CloudEventContentType
{

    /// <summary>
    /// Gets the media type name for cloud events
    /// </summary>
    public const string Prefix = "application/cloudevents";

    /// <summary>
    /// Gets the media type name for cloud events formatted in JSON
    /// </summary>
    public const string Json = Prefix + "+json";

}