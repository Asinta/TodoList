using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Application.TodoLists.Queries.GetTodos;

public class GetTodosQuery : IRequest<List<TodoListBriefDto>>
{
}

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, List<TodoListBriefDto>>
{
    private readonly IRepository<Domain.Entities.TodoList> _repository;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IRepository<Domain.Entities.TodoList> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<TodoListBriefDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        return await _repository
            .GetAsQueryable()
            .AsNoTracking()
            .ProjectTo<TodoListBriefDto>(_mapper.ConfigurationProvider)
            .OrderBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }
}