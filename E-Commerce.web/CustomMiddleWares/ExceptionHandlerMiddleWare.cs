using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.web.CustomMiddleWares
{
    public class ExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleWare> logger;

        public ExceptionHandlerMiddleWare(RequestDelegate next , ILogger<ExceptionHandlerMiddleWare> logger) 
        {
            this.next = next;
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next.Invoke(httpContext);
                if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var Problem = new ProblemDetails()
                    {
                        Title = "Error While Processing The Http Request - EndPoint Not Found",
                        Detail = $"EndPoint {httpContext.Request.Path}not found ",
                        Status = StatusCodes.Status404NotFound,
                        Instance = httpContext.Request.Path
                    };
                    await httpContext.Response.WriteAsJsonAsync(Problem);

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Some thing went wrong");

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var Problem = new ProblemDetails()
                {
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = httpContext.Request.Path
                };
                await httpContext.Response.WriteAsJsonAsync(Problem);
            }
        }
    }
}
