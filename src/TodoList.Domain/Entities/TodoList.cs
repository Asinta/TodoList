using TodoList.Domain.Base;
using TodoList.Domain.Base.Interfaces;
using TodoList.Domain.ValueObjects;

namespace TodoList.Domain.Entities;

public class TodoList : AuditableEntity, IEntity<Guid>, IHasDomainEvent, IAggregateRoot
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
