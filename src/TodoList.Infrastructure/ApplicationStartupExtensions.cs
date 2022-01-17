using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Infrastructure.Identity;
using TodoList.Infrastructure.Persistence;

namespace TodoList.Infrastructure;

public static class ApplicationStartupExtensions
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<TodoListDbContext>();
            await context.Database.MigrateAsync();
            
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await TodoListDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
            
            // 生成种子数据
            await TodoListDbContextSeed.SeedSampleDataAsync(context);
            
            // 更新部分种子数据以便查看审计字段
            await TodoListDbContextSeed.UpdateSampleDataAsync(context);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred migrating the DB: {ex.Message}");
        }
    }
}