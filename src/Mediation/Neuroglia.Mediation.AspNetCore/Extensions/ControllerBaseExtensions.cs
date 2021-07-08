using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Net;

namespace Neuroglia.Mediation
{

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
        public static NotFoundObjectResult NotFound(this ControllerBase controller, ModelStateDictionary commandState)
        {
            return new NotFoundObjectResult(new SerializableError(commandState));
        }

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
            if (result.Code != OperationResultCode.Ok)
            {
                if (result.Code == OperationResultCode.Forbidden)
                    return controller.StatusCode((int)HttpStatusCode.Forbidden);
                if (result.Code == OperationResultCode.Invalid)
                {
                    result.Errors?.ToList().ForEach(e => controller.ModelState.AddModelError(e.Code, e.Message));
                    return controller.BadRequest(controller.ModelState);
                }
                if (result.Code == OperationResultCode.NotFound)
                {
                    result.Errors?.ToList().ForEach(e => controller.ModelState.AddModelError(e.Code, e.Message));
                    return NotFound(controller, controller.ModelState); ;
                }
                if (result.Code == OperationResultCode.NotModified)
                    return controller.StatusCode((int)HttpStatusCode.NotModified);
                return controller.StatusCode((int)HttpStatusCode.InternalServerError);
            }
            if (result.Returned)
                return new ObjectResult(result.Data);
            else
                return controller.StatusCode(successStatusCode);
        }

    }

}
