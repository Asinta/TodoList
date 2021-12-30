using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using TodoList.Application.Common.Interfaces;
using TodoList.Domain.ValueObjects;

namespace TodoList.Application.TodoLists.Commands.CreateTodoList;

public class CreateTodoListCommand : IRequest<Domain.Entities.TodoList>
{
    public string? Title { get; set; }
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