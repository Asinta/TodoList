using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.TodoLists.Specs;

namespace TodoList.Application.TodoLists.Queries.GetSingleTodo;

public class GetSingleTodoQuery : IRequest<TodoListDto?>
{
    public Guid ListId { get; set; }
}

public class ExportTodosQueryHandler : IRequestHandler<GetSingleTodoQuery, TodoListDto?>
{
    private readonly IRepository<Domain.Entities.TodoList> _repository;
    private readonly IMapper _mapper;

    public ExportTodosQueryHandler(IRepository<Domain.Entities.TodoList> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoListDto?> Handle(GetSingleTodoQuery request, CancellationToken cancellationToken)
    {
        var spec = new TodoListSpec(request.ListId, true);
        return await _repository
            .GetAsQueryable(spec)
            .AsNoTracking()
            .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}