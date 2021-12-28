using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using TodoList.Application.Common.Exceptions;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.TodoItems.Queries.GetTodoItems;
using TodoList.Domain.Entities;

namespace TodoList.Application.TodoItems.Commands.PatchTodoItem;

public class PatchTodoItemCommand : IRequest<TodoItem>
{
    public Guid Id { get; set; }
    public JsonPatchDocument<TodoItemDto> Todo { get; set; }
}

public class PatchTodoItemCommandHandler : IRequestHandler<PatchTodoItemCommand, TodoItem>
{
    private readonly IRepository<TodoItem> _repository;
    private readonly IMapper _mapper;

    public PatchTodoItemCommandHandler(IRepository<TodoItem> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoItem> Handle(PatchTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        // 应用Patch
        var todoItemToPatch = _mapper.Map<TodoItemDto>(entity);
        request.Todo.ApplyTo(todoItemToPatch);
        _mapper.Map(todoItemToPatch, entity);
        
        await _repository.SaveChangesAsync(cancellationToken);
        
        return entity;
    }
}