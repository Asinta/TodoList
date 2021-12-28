using AutoMapper;
using TodoList.Application.Common.Mappings;
using TodoList.Domain.Entities;

namespace TodoList.Application.TodoItems.Queries.GetTodoItems;

// 实现IMapFrom<T>接口
public class TodoItemDto : IMapFrom<TodoItem>
{
    public Guid Id { get; set; }

    public Guid ListId { get; set; }

    public string? Title { get; set; }

    public bool Done { get; set; }

    public int Priority { get; set; }
    
    // 实现接口定义的Mapping方法，并提供除了Convention之外的特殊字段的转换规则
    public void Mapping(Profile profile)
    {
        profile.CreateMap<TodoItem, TodoItemDto>()
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => (int)s.Priority));
        
        profile.CreateMap<TodoItem, TodoItemDto>()
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => (int)s.Priority))
            .ReverseMap();
    }
}