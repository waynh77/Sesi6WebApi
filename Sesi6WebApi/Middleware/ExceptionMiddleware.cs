using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sesi6WebApi.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Sesi6WebApi.Middleware
{
    public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);

                var (status, title, detail) = ex switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Not Found", ex.Message),
                    BusinessException => (StatusCodes.Status400BadRequest, "Business Rule Violation", ex.Message),
                    UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message),
                    ValidationException => (StatusCodes.Status400BadRequest, "Validation Error", ex.Message),
                    DbUpdateException => (StatusCodes.Status409Conflict, "Database Conflict", "Data tidak dapat disimpan karena konflik database."),
                    _ => (StatusCodes.Status500InternalServerError, "Server Error", "Terjadi kesalahan pada server.")
                };

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
