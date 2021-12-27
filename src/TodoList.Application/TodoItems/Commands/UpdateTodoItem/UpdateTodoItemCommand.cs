using MediatR;
using TodoList.Application.Common.Exceptions;
using TodoList.Application.Common.Interfaces;
using TodoList.Domain.Entities;

namespace TodoList.Application.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemCommand : IRequest<TodoItem>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Done { get; set; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, TodoItem>
{
    private readonly IRepository<TodoItem> _repository;

    public UpdateTodoItemCommandHandler(IRepository<TodoItem> repository)
    {
        _repository = repository;
    }

    public async Task<TodoItem> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.Title = request.Title ?? entity.Title;
        entity.Done = request.Done;

        await _repository.UpdateAsync(entity, cancellationToken);

        return entity;
    }
}