using System.Dynamic;

namespace TodoList.Application.Common.Interfaces;

public interface IDataShaper<T>
{
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string? fieldString);
    ExpandoObject ShapeData(T entity, string? fieldString);
}