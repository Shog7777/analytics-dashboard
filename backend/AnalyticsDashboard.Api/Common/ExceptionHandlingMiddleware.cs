using System.Net;
using System.Text.Json;

namespace AnalyticsDashboard.Api.Common;

/// <summary>Catches exceptions app-wide and returns a clean JSON error instead of a stack trace.</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Handled API exception: {Message}", ex.Message);
            await WriteError(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(context, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteError(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var payload = JsonSerializer.Serialize(new { error = message, statusCode });
        await context.Response.WriteAsync(payload);
    }
}
