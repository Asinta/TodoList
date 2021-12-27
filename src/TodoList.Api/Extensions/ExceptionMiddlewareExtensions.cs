using TodoList.Api.Middlewares;

namespace TodoList.Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}
