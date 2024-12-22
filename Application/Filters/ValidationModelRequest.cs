using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MindvizServer.Application.Filters
{

    public class ValidationModelFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the model state is valid
            if (!context.ModelState.IsValid)
            {
                var validationErrors = new ValidationProblemDetails(context.ModelState)
                {
                    Title = "Validation Failed",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred."
                };

                // Set the result to a BadRequest with validation details
                context.Result = new BadRequestObjectResult(validationErrors);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No additional action needed after execution
        }
    }

}
