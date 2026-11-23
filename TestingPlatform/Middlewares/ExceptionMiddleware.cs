using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Infrastructure.Exceptions;
namespace TestingPlatform.Middlewares;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            var (status, message) = ex switch
            {
                EntityNotFoundException => (404, ex.Message),
                EntityConflictException => (409, ex.Message),
                ArgumentException => (400, ex.Message),
                DbUpdateException { InnerException: SqliteException { SqliteErrorCode: 19 } }
                    => (409, "Операция нарушает уникальность или связи данных."),
                _ => (500, "Произошла внутренняя ошибка сервера.")
            };
            logger.LogError(ex, "Ошибка запроса {Method} {Path}. Код ответа {StatusCode}, идентификатор {TraceId}",
                context.Request.Method, context.Request.Path, status, context.TraceIdentifier);
            context.Response.Clear();
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { message, statusCode = status, traceId = context.TraceIdentifier });
        }
    }
}
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
