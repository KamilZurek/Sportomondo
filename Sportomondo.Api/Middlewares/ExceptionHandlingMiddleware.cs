using Sportomondo.Api.Exceptions;

namespace Sportomondo.Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException ex)
            {
                var exceptionMessage = "Error: " + ex.Message;

                _logger.LogError(exceptionMessage);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsync(exceptionMessage);
            }
            catch (InvalidTokenException ex)
            {
                var exceptionMessage = "Error: Invalid token";

                _logger.LogError(exceptionMessage);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await context.Response.WriteAsync(exceptionMessage);
            }
            catch (ForbiddenException ex)
            {
                var exceptionMessage = "Error: Forbidden";

                _logger.LogError(exceptionMessage);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                await context.Response.WriteAsync(exceptionMessage);
            }
            catch (NotFoundException ex)
            {
                var exceptionMessage = "Error: " + ex.Message;

                _logger.LogError(exceptionMessage);

                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await context.Response.WriteAsync(exceptionMessage);
            }
            catch (OperationCanceledException ex)
            {
                var exceptionMessage = "Error: Operation was canceled. " + ex.Message;

                _logger.LogError(exceptionMessage);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsync(exceptionMessage);
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Something went wrong. " + ex.Message;

                _logger.LogError(exceptionMessage);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsync(exceptionMessage);
            }
        }
    }
}
