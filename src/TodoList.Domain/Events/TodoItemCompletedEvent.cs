using TodoList.Domain.Base;
using TodoList.Domain.Entities;

namespace TodoList.Domain.Events;

public class TodoItemCompletedEvent : DomainEvent
{
    public TodoItemCompletedEvent(TodoItem item) => Item = item;

    public TodoItem Item { get; }
}
