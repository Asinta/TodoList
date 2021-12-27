using TodoList.Application.Common.Mappings;

namespace TodoList.Application.TodoLists.Queries.GetTodos;

// 实现IMapFrom<T>接口，因为此Dto不涉及特殊字段的Mapping规则
// 并且属性名称与领域实体保持一致，根据Convention规则默认可以完成Mapping，不需要额外实现
public class TodoListBriefDto : IMapFrom<Domain.Entities.TodoList>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Colour { get; set; }
}