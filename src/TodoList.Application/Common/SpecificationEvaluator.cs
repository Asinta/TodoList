using TodoList.Application.Common.Interfaces;

namespace TodoList.Application.Common;

public class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T>? specification)
    {
        var query = inputQuery;

        if (specification?.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification?.Include is not null)
        {
            query = specification.Include(query);
        }

        if (specification?.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification?.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification?.IsPagingEnabled != false)
        {
            query = query.Skip(specification!.Skip).Take(specification.Take);
        }
        
        return query;
    }
}