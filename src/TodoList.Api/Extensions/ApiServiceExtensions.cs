using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using TodoList.Api.Controllers;

namespace TodoList.Api.Extensions;

public static class ApiServiceExtensions
{
    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // 向响应头中添加API版本信息
            options.ReportApiVersions = true;
            // 如果客户端不显式指定API版本，则使用默认版本
            options.AssumeDefaultVersionWhenUnspecified = true;
            // 配置默认版本为1.0
            options.DefaultApiVersion = new ApiVersion(1, 0);
            // 指定请求头中携带用于指定API版本信息的字段
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            // 使用Convention标记deprecated
            options.Conventions.Controller<TodoItemV2Controller>().HasDeprecatedApiVersion(new ApiVersion(2, 0));
        });
    }
}