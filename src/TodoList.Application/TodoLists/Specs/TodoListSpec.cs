using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common;

namespace TodoList.Application.TodoLists.Specs;

public sealed class TodoListSpec : SpecificationBase<Domain.Entities.TodoList>
{
    public TodoListSpec(Guid id, bool includeItems = false) : base(t => t.Id == id)
    {
        if (includeItems)
        {
            AddInclude(t => t.Include(i => i.Items));
        }
    }
}
