using System.Text.Json;
using Google;
using Vostok.Logging.Abstractions;

namespace CookingBot;

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
        context.Response.StatusCode = 200;

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