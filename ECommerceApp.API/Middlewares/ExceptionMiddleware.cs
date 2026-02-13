using ECommerceApp.Core.Exceptions;
using ECommerceApp.Core.Models;
using System.Text.Json;

namespace ECommerceApp.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception exception)
            {
                var (statusCode, errors) = exception switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, null),
                    BadRequestException => (StatusCodes.Status400BadRequest, null),
                    ValidationException e => (StatusCodes.Status400BadRequest, (object)e.Errors),
                    UnauthorizedException => (StatusCodes.Status401Unauthorized, null),
                    ForbiddenException => (StatusCodes.Status403Forbidden, null),
                    _ => (StatusCodes.Status500InternalServerError, null)
                };

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = statusCode;
                
                var errorDetails = new ErrorDetails
                {
                    StatusCode = statusCode,
                    Message = exception.Message,
                    Path = httpContext.Request.Path,
                    Timestamp = DateTime.Now,
                    TraceId = httpContext.TraceIdentifier,
                    Errors = errors
                };

                var jsonResponse = JsonSerializer.Serialize(errorDetails);
                await httpContext.Response.WriteAsync(jsonResponse);

            }
        }
    }
}
