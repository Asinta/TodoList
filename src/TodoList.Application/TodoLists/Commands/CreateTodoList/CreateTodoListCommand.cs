using MediatR;
using TodoList.Application.Common.Interfaces;
using TodoList.Domain.ValueObjects;

namespace TodoList.Application.TodoLists.Commands.CreateTodoList;

/// <summary>
/// 创建TodoList指令对象
/// </summary>
public class CreateTodoListCommand : IRequest<Domain.Entities.TodoList>
{
    /// <summary>
    /// TodoList的名称
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// TodoList使用的主题颜色
    /// </summary>
    public string? Colour { get; set; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, Domain.Entities.TodoList>
{
    private readonly IRepository<Domain.Entities.TodoList> _repository;

    public CreateTodoListCommandHandler(IRepository<Domain.Entities.TodoList> repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Entities.TodoList> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.TodoList
        {
            Title = request.Title,
            Colour = Colour.From(request.Colour ?? string.Empty)
        };

        await _repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}