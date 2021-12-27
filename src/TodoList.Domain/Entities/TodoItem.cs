using TodoList.Domain.Base;
using TodoList.Domain.Base.Interfaces;
using TodoList.Domain.Enums;
using TodoList.Domain.Events;

namespace TodoList.Domain.Entities;

public class TodoItem : AuditableEntity, IEntity<Guid>, IHasDomainEvent
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public PriorityLevel Priority { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && _done == false)
            {
                DomainEvents.Add(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public Guid ListId { get; set; }
    public TodoList List { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
