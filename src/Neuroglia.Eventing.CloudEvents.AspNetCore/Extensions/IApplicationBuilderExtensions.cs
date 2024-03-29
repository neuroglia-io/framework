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

using Microsoft.AspNetCore.Builder;

namespace Neuroglia.Eventing.CloudEvents.AspNetCore;

/// <summary>
/// Defines extensions for <see cref="IApplicationBuilder"/>s
/// </summary>
public static class CloudEventIApplicationBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="IApplicationBuilder"/> to use the <see cref="CloudEventMiddleware"/>
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IApplicationBuilder"/></returns>
    public static IApplicationBuilder UseCloudEvents(this IApplicationBuilder app) => app.UseMiddleware<CloudEventMiddleware>();

}
