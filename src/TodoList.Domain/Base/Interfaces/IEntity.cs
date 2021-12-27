namespace TodoList.Domain.Base.Interfaces;

public interface IEntity<T>
{
    public T Id { get; set; }
}