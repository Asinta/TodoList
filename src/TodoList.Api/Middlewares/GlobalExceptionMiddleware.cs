using System.Net;
using TodoList.Api.Models;
using TodoList.Application.Common.Exceptions;

namespace TodoList.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
        
    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            // 你可以在这里进行相关的日志记录
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // 对于抛出异常来说，更好的方式是在Application层定义一些继承自Exception的自定义异常
        // 然后在这里进行判断，这里只是做了简单的演示
        context.Response.StatusCode = exception switch
        {
            ApplicationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var responseModel = ApiResponse<string>.Fail(exception.Message);

        await context.Response.WriteAsync(responseModel.ToJsonString());
    }
}