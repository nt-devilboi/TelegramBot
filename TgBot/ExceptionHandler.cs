using System.Text.Json;
using Google;
using Vostok.Logging.Abstractions;

namespace TgBot;

public class ExceptionHandlingMiddleware
{
    private readonly ILog _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILog log)
    {
        _next = next;
        log.ForContext("ExceptionMHandlingMiddleware");
        _logger = log;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Warn($"ошибка: {ex}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            GoogleApiException googleEx => (int)googleEx.HttpStatusCode,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            error = exception.GetType().Name,
            message = exception.Message,
            stackTrace = context.RequestServices.GetService<IHostEnvironment>().IsDevelopment()
                ? exception.StackTrace
                : null
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}