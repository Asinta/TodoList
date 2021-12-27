using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;

namespace TodoList.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Domain.Entities.TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}