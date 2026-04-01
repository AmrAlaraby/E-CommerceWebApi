using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.web.Factories
{
    public static class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationResponse(ActionContext actionContext)
        {
            var Errors = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(x => x.Key, x => x.Value.Errors.Select(x => x.ErrorMessage).ToArray());
            var Problem = new ProblemDetails()
            {
                Title = "Validation Error",
                Detail = "One or more validation errors occured.",
                Status = StatusCodes.Status400BadRequest,
                Extensions =
                        {
                            {"Errors" , Errors}
                        }
            };
            return new BadRequestObjectResult(Problem);
        }
    }
}
