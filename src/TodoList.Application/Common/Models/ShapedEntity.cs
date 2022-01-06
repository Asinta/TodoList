using TodoList.Domain.Base.Interfaces;

namespace TodoList.Application.Common.Models;

public class ShapedEntity<TKey>
{
    public TKey Id { get; set; }
    public IEntity<TKey> Entity { get; set; }
    
    public ShapedEntity()
    {
    }
}