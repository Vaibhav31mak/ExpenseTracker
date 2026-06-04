namespace ExpenseTracker.API.Middleware;

// Handling internal error 500.
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception not handled.");

            var problem = Results.Problem(
                title: "An error happened",
                detail: "An error happened. Check console for details",
                statusCode: StatusCodes.Status500InternalServerError);

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await problem.ExecuteAsync(context);
        }
    }
}
