using MediatR;
using Microsoft.Extensions.Logging;
using TodoList.Application.Common.Models;
using TodoList.Domain.Events;

namespace TodoList.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler : INotificationHandler<DomainEventNotification<TodoItemCompletedEvent>>
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<TodoItemCompletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // 这里我们还是只做日志输出，实际使用中根据需要进行业务逻辑处理，但是在Handler中不建议继续Send其他Command或Notification
        _logger.LogInformation("TodoList Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}