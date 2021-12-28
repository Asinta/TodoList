using TodoList.Api.Extensions;
using TodoList.Application;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Log;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureLog();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseAuthorization();
app.MapControllers();

app.MigrateDatabase();

app.Run();