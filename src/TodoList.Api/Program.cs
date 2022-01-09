using System.Reflection;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using TodoList.Api.Extensions;
using TodoList.Api.Filters;
using TodoList.Application;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Log;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureLog();
builder.Services.ConfigureApiVersioning();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
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
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("1.0", new OpenApiInfo { Title = "TodoList API", Version = "1.0"});
    s.SwaggerDoc("2.0", new OpenApiInfo { Title = "TodoList API", Version = "2.0"});
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
    
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Add JWT with Bearer",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer"
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<LogFilterAttribute>();

// 添加应用层配置
builder.Services.AddApplication();
// 添加基础设施配置
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
// app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/1.0/swagger.json", "TodoList API v1.0");
        s.SwaggerEndpoint("/swagger/2.0/swagger.json", "TodoList API v2.0");
    });
}

app.UseHttpLogging();
// app.UseResponseCaching();
// app.UseHttpCacheHeaders();
var supportedCultures = new[] { "en-US", "zh" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.MapDefaultControllerRoute();
app.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
app.MapHealthChecks("/hc");

app.MigrateDatabase();

app.Run();