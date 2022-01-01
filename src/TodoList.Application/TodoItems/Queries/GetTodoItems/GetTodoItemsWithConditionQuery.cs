using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.Common.Mappings;
using TodoList.Application.Common.Models;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.TodoItems.Queries.GetTodoItems;

public class GetTodoItemsWithConditionQuery : IRequest<PaginatedList<TodoItemDto>>
{
    public Guid ListId { get; set; }
    public bool? Done { get; set; }
    public PriorityLevel? PriorityLevel { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetTodoItemsWithConditionQueryHandler : IRequestHandler<GetTodoItemsWithConditionQuery, PaginatedList<TodoItemDto>>
{
    private readonly IRepository<TodoItem> _repository;
    private readonly IMapper _mapper;

    public GetTodoItemsWithConditionQueryHandler(IRepository<TodoItem> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemDto>> Handle(GetTodoItemsWithConditionQuery request, CancellationToken cancellationToken)
    {
        return await _repository
            .GetAsQueryable(x => x.ListId == request.ListId 
                                 && (!request.Done.HasValue || x.Done == request.Done) 
                                 && (!request.PriorityLevel.HasValue || x.Priority == request.PriorityLevel))
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}