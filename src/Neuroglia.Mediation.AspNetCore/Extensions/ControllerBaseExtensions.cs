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
        if (result.Status != (int)HttpStatusCode.OK)
        {
            if (result.Status == (int)HttpStatusCode.Forbidden) return controller.StatusCode((int)HttpStatusCode.Forbidden);
            if (result.Status == (int)HttpStatusCode.BadRequest)
            {
                result.Errors?.ToList().ForEach(e => controller.ModelState.AddModelError(e.Title!, e.Detail!));
                return controller.ValidationProblem(controller.ModelState);
            }
            if (result.Status == (int)HttpStatusCode.NotFound)
            {
                result.Errors?.ToList().ForEach(e => controller.ModelState.AddModelError(e.Title!, e.Detail!));
                return NotFound(controller, controller.ModelState); ;
            }
            if (result.Status == (int)HttpStatusCode.NotModified) return controller.StatusCode((int)HttpStatusCode.NotModified);
            return controller.StatusCode((int)HttpStatusCode.InternalServerError);
        }
        if (result.Data != null) return new ObjectResult(result.Data) { StatusCode = successStatusCode };
        else return controller.StatusCode(successStatusCode);
    }

}
