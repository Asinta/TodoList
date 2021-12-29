using System.Reflection;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest>> _logger;

    // 在构造函数中后面我们还可以注入类似ICurrentUser和IIdentity相关的对象进行日志输出
    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest>> logger)
    {
        _logger = logger;
    }
    
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        // 你可以在这里log关于请求的任何信息
        _logger.LogInformation($"Handling {typeof(TRequest).Name}");
        
        IList<PropertyInfo> props = new List<PropertyInfo>(request.GetType().GetProperties());
        foreach (var prop in props)
        {
            var propValue = prop.GetValue(request, null);
            _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
        }
    }
}