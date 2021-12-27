using TodoList.Domain.Base;

namespace TodoList.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
