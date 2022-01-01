using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.Common.Mappings;
using TodoList.Application.Common.Models;
using TodoList.Application.TodoItems.Specs;
using TodoList.Domain.Entities;

namespace TodoList.Application.TodoItems.Queries.GetTodoItems;

public class GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemDto>>
{
    public Guid ListId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemDto>>
{
    private readonly IRepository<TodoItem> _repository;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IRepository<TodoItem> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _repository
            .GetAsQueryable(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}