using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoList.Application.Common;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Infrastructure.Persistence.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    private readonly TodoListDbContext _dbContext;

    public RepositoryBase(TodoListDbContext dbContext) => _dbContext = dbContext;

    public virtual ValueTask<T?> GetAsync(object key) => _dbContext.Set<T>().FindAsync(key);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        // 对于一般的更新而言，都是Attach到实体上的，只需要设置该实体的State为Modified就可以了
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(object key)
    {
        var entity = await GetAsync(key);
        if (entity is not null)
        {
            await DeleteAsync(entity);
        }
    }
    
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    // 1. 查询基础操作接口实现
    public IQueryable<T> GetAsQueryable() 
        => _dbContext.Set<T>();

    public IQueryable<T> GetAsQueryable(ISpecification<T> spec)
        => ApplySpecification(spec);
    public IQueryable<T> GetAsQueryable(Expression<Func<T, bool>> condition)
        => _dbContext.Set<T>().Where(condition);

    // 2. 查询数量相关接口实现
    public int Count(Expression<Func<T, bool>> condition) 
        => _dbContext.Set<T>().Count(condition);
    public int Count(ISpecification<T>? spec = null) 
        => null != spec ? ApplySpecification(spec).Count() : _dbContext.Set<T>().Count();
    public Task<int> CountAsync(ISpecification<T>? spec) 
        => ApplySpecification(spec).CountAsync();

    // 3. 查询存在性相关接口实现
    public bool Any(ISpecification<T>? spec) 
        => ApplySpecification(spec).Any();
    public bool Any(Expression<Func<T, bool>>? condition = null) 
        => null != condition ? _dbContext.Set<T>().Any(condition) : _dbContext.Set<T>().Any();
    
    // 4. 根据条件获取原始实体类型数据相关接口实现
    public async Task<T?> GetAsync(Expression<Func<T, bool>> condition) 
        => await _dbContext.Set<T>().FirstOrDefaultAsync(condition);
    public async Task<IReadOnlyList<T>> GetAsync() 
        => await _dbContext.Set<T>().AsNoTracking().ToListAsync();
    public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T>? spec) 
        => await ApplySpecification(spec).AsNoTracking().ToListAsync();

    // 5. 根据条件获取映射实体类型数据相关接口实现
    public TResult? SelectFirstOrDefault<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector) 
        => ApplySpecification(spec).AsNoTracking().Select(selector).FirstOrDefault();
    public Task<TResult?> SelectFirstOrDefaultAsync<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector) 
        => ApplySpecification(spec).AsNoTracking().Select(selector).FirstOrDefaultAsync();
    
    public async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector) 
        => await _dbContext.Set<T>().AsNoTracking().Select(selector).ToListAsync();
    public async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(ISpecification<T>? spec, Expression<Func<T, TResult>> selector) 
        => await ApplySpecification(spec).AsNoTracking().Select(selector).ToListAsync();
    public async Task<IReadOnlyList<TResult>> SelectAsync<TGroup, TResult>(
        Expression<Func<T, TGroup>> groupExpression,
        Expression<Func<IGrouping<TGroup, T>, TResult>> selector,
        ISpecification<T>? spec = null) 
        => null != spec ?
            await ApplySpecification(spec).AsNoTracking().GroupBy(groupExpression).Select(selector).ToListAsync() :
            await _dbContext.Set<T>().AsNoTracking().GroupBy(groupExpression).Select(selector).ToListAsync();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    private IQueryable<T> ApplySpecification(ISpecification<T>? spec) 
        => SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
}