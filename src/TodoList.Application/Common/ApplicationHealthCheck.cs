using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TodoList.Application.Common;

public class ApplicationHealthCheck : IHealthCheck
{
    private static readonly Random _rnd = new ();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = _rnd.Next(5) == 0
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Failed random");

        return Task.FromResult(result);
    }
}