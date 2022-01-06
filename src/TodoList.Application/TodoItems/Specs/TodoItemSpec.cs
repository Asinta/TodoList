using TodoList.Application.Common;
using TodoList.Application.TodoItems.Queries.GetTodoItems;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.TodoItems.Specs;

public sealed class TodoItemSpec : SpecificationBase<TodoItem>
{
    public TodoItemSpec(bool done, PriorityLevel priority) : base(t => t.Done == done && t.Priority == priority)
    {
    }
    
    public TodoItemSpec(Guid id) : base(t => t.Id == id)
    {
    }
    
    public TodoItemSpec(GetTodoItemsWithConditionQuery query) : 
        base(x => x.ListId == query.ListId 
                  && (!query.Done.HasValue || x.Done == query.Done) 
                  && (!query.PriorityLevel.HasValue || x.Priority == query.PriorityLevel)
                  && (string.IsNullOrEmpty(query.Title) || x.Title!.Trim().ToLower().Contains(query.Title!.ToLower())))
    {
        if (string.IsNullOrEmpty(query.SortOrder))
            return;

        switch (query.SortOrder)
        {
            // 仅作有限的演示
            default:
                ApplyOrderBy(x => x.Title!);
                break;
            case "title_desc":
                ApplyOrderByDescending(x =>x .Title!);
                break;
            case "priority_asc":
                ApplyOrderBy(x => x.Priority);
                break;
            case "priority_desc": 
                ApplyOrderByDescending(x => x.Priority); 
                break;
        }
    }
}