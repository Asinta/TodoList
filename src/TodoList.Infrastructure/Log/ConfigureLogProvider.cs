using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TodoList.Infrastructure.Log;

public static class ConfigureLogProvider
{
    public static void ConfigureLog(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<bool>("UseFileToLog"))
        {
            // 配置同时输出到控制台和文件，并且指定文件名和文件转储方式（形如log-20211219.txt格式），转储文件保留的天数为15天，以及日志格式
            // 配置Enrich.FromLogContext()的目的是为了从日志上下文中获取一些关键信息诸如用户ID或请求ID，我们的应用中暂时不使用这些。
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/log-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 15)
                .CreateLogger();
        }
        else
        {
            // 仅配置控制台日志
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        // 使用Serilog作为日志框架
        builder.Host.UseSerilog();
    }
}