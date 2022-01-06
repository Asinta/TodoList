using System.Dynamic;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Application.Common.Extensions;

public static class DataShaperExtensions
{
    public static IEnumerable<ExpandoObject> ShapeData<T>(this IEnumerable<T> entities, IDataShaper<T> shaper, string? fieldString)
    {
        return shaper.ShapeData(entities, fieldString);
    }
}