using E_Commerce.Shared.CommonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        // handle result without value 
        // if result is success Return NoContent 204
        // if result is Failure Return Problem With status code and Details

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess) //True
                return NoContent(); //204
            else
                return HandleProblem(result.Errors);
        }

        // handle result with value
        // if result is success Return Ok 200 with value
        // if result is Failure Return Problem With status code and Details

        protected ActionResult<TValue> HandleResult<TValue>(Result<TValue> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return HandleProblem(result.Errors);

        }

        private ActionResult HandleProblem(IReadOnlyList<Error> errors)
        {
            // If No Errors Are Provided , Return 500 Error
            if (errors.Count == 0)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected Error Occurred");
            }
            // If All Errors are Validation Errors , Handle Them As Validation Problem
            if (errors.All(X => X.Type == ErrorType.Validation))
            {
                return HandleValidationProblem(errors);
            }
            // If There`s Only One Error , Handle It As A Single Error Problem
            return HandleSingleErrorProblem(errors[0]);
            
        }
        private ActionResult HandleSingleErrorProblem(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapErrorTypeToStatusCode(error.Type)
                );
        }
        private static int MapErrorTypeToStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.UnAuthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };
        } 
        private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelState);
        }
    }
}
