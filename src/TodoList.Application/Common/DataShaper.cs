using System.Dynamic;
using System.Reflection;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Application.Common;

public class DataShaper<T> : IDataShaper<T> where T : class
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string? fieldString)
    {
        var requiredProperties = GetRequiredProperties(fieldString);

        return GetData(entities, requiredProperties);
    }
    
    public ExpandoObject ShapeData(T entity, string? fieldString)
    {
        var requiredProperties = GetRequiredProperties(fieldString);

        return GetDataForEntity(entity, requiredProperties);
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(string? fieldString)
    {
        var requiredProperties = new List<PropertyInfo>();

        if (!string.IsNullOrEmpty(fieldString))
        {
            var fields = fieldString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields)
            {
                var property = Properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (property == null)
                {
                    continue;
                }

                requiredProperties.Add(property);
            }
        }
        else
        {
            requiredProperties = Properties.ToList();
        }

        return requiredProperties;
    }
    
    private IEnumerable<ExpandoObject> GetData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
    {
        return entities.Select(entity => GetDataForEntity(entity, requiredProperties)).ToList();
    }

    private ExpandoObject GetDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedObject = new ExpandoObject();
        foreach (var property in requiredProperties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.TryAdd(property.Name, objectPropertyValue);
        }

        return shapedObject;
    }
}