using Sportomondo.Api.Exceptions;

namespace Sportomondo.Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = 400;

                await context.Response.WriteAsync("Error: " + ex.Message);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;

                await context.Response.WriteAsync("Error: " + ex.Message);
            }         
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                await context.Response.WriteAsync("Something went wrong" + Environment.NewLine 
                    + Environment.NewLine + ex.Message);
            }
        }
    }
}
