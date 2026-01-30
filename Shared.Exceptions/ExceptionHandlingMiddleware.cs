using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseCustomException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                Status = ex.StatusCode,
                Code = ex.ErrorCode,
                Message = ex.Message,
                Description = ex.Description,
                TraceId = context.TraceIdentifier,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                Status = 500,
                Code = "INTERNAL_SERVER_ERROR",
                Message = "Something went wrong",
                Description = "An unexpected error occurred",
                TraceId = context.TraceIdentifier,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
