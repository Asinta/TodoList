using AspNetCoreRateLimit;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Extensions;
using TodoList.Api.Filters;
using TodoList.Application;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Log;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureLog();
builder.Services.ConfigureApiVersioning();
builder.Services.AddResponseCaching();
builder.Services.AddHttpCacheHeaders(
    expirationOptions =>
    {
        expirationOptions.MaxAge = 180;
        expirationOptions.CacheLocation = CacheLocation.Private;
    },
    validateOptions =>
    {
        validateOptions.MustRevalidate = true;
    });
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("60SecondDuration", new CacheProfile { Duration = 60 });
});
builder.Services.AddHttpLogging(options =>
{
    // 日志记录的字段配置，可以以 | 连接
    options.LoggingFields = HttpLoggingFields.All;
    // 增加请求头字段记录
    options.RequestHeaders.Add("Sec-Fetch-Site");
    options.RequestHeaders.Add("Sec-Fetch-Mode");
    options.RequestHeaders.Add("Sec-Fetch-Dest");
    // 增加响应头字段记录
    options.ResponseHeaders.Add("Server");
    // 增加请求的媒体类型
    options.MediaTypeOptions.AddText("application/javascript");
    // 配置请求体日志最大长度
    options.RequestBodyLogLimit = 4096;
    // 配置响应体日志最大长度
    options.ResponseBodyLogLimit = 4096;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<LogFilterAttribute>();

// 添加应用层配置
builder.Services.AddApplication();
// 添加基础设施配置
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseAuthorization();
app.UseHttpLogging();
app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.MapControllers();

app.MigrateDatabase();

app.Run();