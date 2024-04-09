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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Neuroglia.Mediation.AspNetCore;

/// <summary>
/// Defines extensions for <see cref="ControllerBase"/>s
/// </summary>
public static class ControllerBaseExtensions
{

    /// <summary>
    /// Creates a new <see cref="NotFoundObjectResult"/> based on the specified <see cref="ModelStateDictionary"/>
    /// </summary>
    /// <param name="controller">The extended <see cref="ControllerBase"/></param>
    /// <param name="commandState">The <see cref="ModelStateDictionary"/> from which to create the <see cref="NotFoundObjectResult"/></param>
    /// <returns>A new <see cref="NotFoundObjectResult"/> based on the specified <see cref="ModelStateDictionary"/></returns>
    public static NotFoundObjectResult NotFound(this ControllerBase controller, ModelStateDictionary commandState) => new(new SerializableError(commandState));

    /// <summary>
    /// Processes the specified <see cref="IOperationResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type of <see cref="IOperationResult"/> to process</typeparam>
    /// <param name="controller">The extended <see cref="ControllerBase"/></param>
    /// <param name="result">The <see cref="IOperationResult"/> to process</param>
    /// <param name="successStatusCode">The <see cref="HttpStatusCode"/> to return in case of success. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    public static ActionResult Process<TResult>(this ControllerBase controller, TResult result, int successStatusCode = 200)
        where TResult : IOperationResult
    {
        if (!(result.Status >= 200 && result.Status < 300))
        {
            if (result.Status == (int)HttpStatusCode.Forbidden) return controller.StatusCode((int)HttpStatusCode.Forbidden);
            if (result.Status == (int)HttpStatusCode.BadRequest)
            {
                result.Errors?.ToList().SelectMany(e => e.Errors == null ? new Dictionary<string, string[]>() { { e.Title!, new string[] { e.Detail! } } }.ToList() : e.Errors.ToList()).ToList().ForEach(e => controller.ModelState.AddModelError(e.Key, string.Join(Environment.NewLine, e.Value)));
                return controller.ValidationProblem(controller.ModelState);
            }
            if (result.Status == (int)HttpStatusCode.NotFound)
            {
                result.Errors?.ToList().ForEach(e => controller.ModelState.AddModelError(e.Title!, e.Detail!));
                return NotFound(controller, controller.ModelState);
            }
            if (result.Status == (int)HttpStatusCode.NotModified) return controller.StatusCode((int)HttpStatusCode.NotModified);
            return controller.StatusCode((int)HttpStatusCode.InternalServerError, result.Data);
        }
        if (result.Data != null) return new ObjectResult(result.Data) { StatusCode = successStatusCode };
        else return controller.StatusCode(successStatusCode);
    }

}
