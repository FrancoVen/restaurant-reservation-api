using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.API.Errors
{
    public static class ErrorMapping
    {
        public static int ToStatusCode(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
        }


        public static ProblemDetails ToProblemDetails(List<Error> errors, HttpContext httpContext)
        {
            var firstError = errors.First();

            return new ProblemDetails()
            {
                Title = $"{firstError.Code}",
                Status = ToStatusCode(firstError),
                Detail = string.Join(" — ", errors.Select(e => e.Description)),
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };
        }


        public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, List<Error> errors)
        {
            var problem = ErrorMapping.ToProblemDetails(errors, controller.HttpContext);
            return controller.StatusCode(problem.Status ?? 500, problem);
        }

        public static IActionResult ToActionResult(this ControllerBase controller, List<Error> errors)
        {
            var problem = ErrorMapping.ToProblemDetails(errors, controller.HttpContext);
            return controller.StatusCode(problem.Status ?? 500, problem);
        }
    }
}
