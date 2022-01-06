using AspNetCoreRateLimit;

namespace TodoList.Api.Extensions;

public static class RateLimitingServiceExtensions
{
    public static void ConfigureRateLimiting(this IServiceCollection services)
    {
        // 当然也可以使用Configuration的方式读取RateLimit规则
        var rateLimitRules = new List<RateLimitRule>
        {
            new ()
            {
                Endpoint = "*",
                Limit = 2,
                Period = "5m"
            }
        };
        services.Configure<IpRateLimitOptions>(options => options.GeneralRules = rateLimitRules);

        // 使用内存作为存储
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }
}