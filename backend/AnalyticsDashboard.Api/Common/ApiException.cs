namespace AnalyticsDashboard.Api.Common;

/// <summary>Business-level error (not found, conflict, bad input). Caught by ExceptionHandlingMiddleware and returned as JSON.</summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public static ApiException NotFound(string entity, object key) =>
        new(404, $"{entity} with id '{key}' was not found.");

    public static ApiException Conflict(string message) => new(409, message);

    public static ApiException BadRequest(string message) => new(400, message);
}
