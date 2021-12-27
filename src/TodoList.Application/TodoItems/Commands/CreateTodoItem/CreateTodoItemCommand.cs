using MediatR;
using TodoList.Application.Common.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Events;

namespace TodoList.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommand : IRequest<TodoItem>
{
    public Guid ListId { get; set; }

    public string? Title { get; set; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, TodoItem>
{
    private readonly IRepository<TodoItem> _repository;

    public CreateTodoItemCommandHandler(IRepository<TodoItem> repository)
    {
        _repository = repository;
    }

    public async Task<TodoItem> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            // 这个ListId在前文中的代码里漏掉了，需要添加到Domain.Entities.TodoItem实体上
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        // 添加领域事件
        entity.DomainEvents.Add(new TodoItemCreatedEvent(entity));
        
        await _repository.AddAsync(entity, cancellationToken);

        return entity;
    }
}