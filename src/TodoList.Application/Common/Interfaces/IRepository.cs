using System.Linq.Expressions;

namespace TodoList.Application.Common.Interfaces;

public interface IRepository<T> where T : class
{
    // 1. 查询基础操作接口
    IQueryable<T> GetAsQueryable();
    IQueryable<T> GetAsQueryable(ISpecification<T> spec);
    
    // 2. 查询数量相关接口
    int Count(ISpecification<T>? spec = null);
    int Count(Expression<Func<T, bool>> condition);
    Task<int> CountAsync(ISpecification<T>? spec);

    // 3. 查询存在性相关接口
    bool Any(ISpecification<T>? spec);
    bool Any(Expression<Func<T, bool>>? condition = null);
    
    // 4. 根据条件获取原始实体类型数据相关接口
    ValueTask<T?> GetAsync(object key);
    Task<T?> GetAsync(Expression<Func<T, bool>> condition);
    Task<IReadOnlyList<T>> GetAsync();
    Task<IReadOnlyList<T>> GetAsync(ISpecification<T>? spec);

    // 5. 根据条件获取映射实体类型数据相关接口，涉及到Group相关操作也在其中
    TResult? SelectFirstOrDefault<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector);
    Task<TResult?> SelectFirstOrDefaultAsync<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector);
    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<IReadOnlyList<TResult>> SelectAsync<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector);
    Task<IReadOnlyList<TResult>> SelectAsync<TGroup, TResult>(Expression<Func<T, TGroup>> groupExpression, Expression<Func<IGrouping<TGroup, T>, TResult>> selector, ISpecification<T>? spec = null);

    // Create相关操作接口
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    // Update相关操作接口
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    // Delete相关操作接口
    Task DeleteAsync(object key);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}