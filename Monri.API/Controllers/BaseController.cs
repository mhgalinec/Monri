using Microsoft.AspNetCore.Mvc;
using Monri.Core.Models;

namespace Monri.API.Controllers
{
    public abstract class BaseController<T>() : ControllerBase where T : BaseController<T>
    {
        protected ObjectResult BadResult(Error error)
        {
            return error.Code switch
            {
                "400" => StatusCode(StatusCodes.Status400BadRequest, error.Message),
                "401" => StatusCode(StatusCodes.Status401Unauthorized, error.Message),
                "403" => StatusCode(StatusCodes.Status403Forbidden, error.Message),
                "404" => StatusCode(StatusCodes.Status404NotFound, error.Message),
                "500" => StatusCode(StatusCodes.Status500InternalServerError, error.Message),
                "R00" => StatusCode(StatusCodes.Status429TooManyRequests, error.Message),
                "D00" => StatusCode(StatusCodes.Status409Conflict, error.Message),
                _ => BadRequest(error)
            };
        }
    }
}
