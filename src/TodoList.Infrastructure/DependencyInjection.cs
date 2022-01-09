using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TodoList.Application.Common.Configurations;
using TodoList.Application.Common.Interfaces;
using TodoList.Infrastructure.Identity;
using TodoList.Infrastructure.Persistence;
using TodoList.Infrastructure.Persistence.Repositories;
using TodoList.Infrastructure.Services;

namespace TodoList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TodoListDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServerConnection"),
                b => b.MigrationsAssembly(typeof(TodoListDbContext).Assembly.FullName)));

        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));

        // 配置内建的IdentityServer用于API授权
        // services
        //     .AddIdentityServer()
        //     .AddDeveloperSigningCredential()
        //     .AddApiAuthorization<ApplicationUser, TodoListDbContext>();
        
        // 配置认证服务
        services
            .AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = 6;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<TodoListDbContext>()
            .AddDefaultTokenProviders();

        // 注入认证服务
        services.AddTransient<IIdentityService, IdentityService>();

        // 增加依赖注入
        services.AddScoped<IDomainEventService, DomainEventService>();
        
        // 添加认证方法为JWT Token认证
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

        // 使用IOptions配置
        services.Configure<JwtConfiguration>("JwtSettings", configuration.GetSection("JwtSettings"));
        services.Configure<JwtConfiguration>("JwtApiV2Settings", configuration.GetSection("JwtApiV2Settings"));
        
        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // 改为使用配置类成员获取
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    ValidAudience = jwtConfiguration.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET") ?? "TodoListApiSecretKey"))
                };
            });

        // 添加授权Policy是基于角色的，策略名称为OnlyAdmin，策略要求具有Administrator角色
        services.AddAuthorization(options =>
        {
            options.AddPolicy("OnlyAdmin", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("OnlySuper", policy => policy.RequireRole("SuperAdmin"));
        });
        
        return services;
    } 
}