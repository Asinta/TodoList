using TodoList.Domain.Base;
using TodoList.Domain.Entities;

namespace TodoList.Domain.Events;

public class TodoItemCreatedEvent : DomainEvent
{
    public TodoItemCreatedEvent(TodoItem item)
        => Item = item;
    public TodoItem Item { get; }
}