using System.Dynamic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using TodoList.Application.Common.Extensions;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.Common.Mappings;
using TodoList.Application.Common.Models;
using TodoList.Application.TodoItems.Specs;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.TodoItems.Queries.GetTodoItems;

public class GetTodoItemsWithConditionQuery : IRequest<PaginatedList<ExpandoObject>>
{
    public Guid ListId { get; set; }
    public bool? Done { get; set; }
    public string? Title { get; set; }
    // 前端指明需要返回的字段
    public string? Fields { get; set; }
    public PriorityLevel? PriorityLevel { get; set; }
    public string? SortOrder { get; set; } = "title_asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetTodoItemsWithConditionQueryHandler : IRequestHandler<GetTodoItemsWithConditionQuery, PaginatedList<ExpandoObject>>
{
    private readonly IRepository<TodoItem> _repository;
    private readonly IMapper _mapper;
    private readonly IDataShaper<TodoItemDto> _shaper;

    public GetTodoItemsWithConditionQueryHandler(IRepository<TodoItem> repository, IMapper mapper, IDataShaper<TodoItemDto> shaper)
    {
        _repository = repository;
        _mapper = mapper;
        _shaper = shaper;
    }

    public Task<PaginatedList<ExpandoObject>> Handle(GetTodoItemsWithConditionQuery request, CancellationToken cancellationToken)
    {
        var spec = new TodoItemSpec(request);
        return Task.FromResult(
            _repository
                .GetAsQueryable(spec)
                .ProjectTo<TodoItemDto>(_mapper.ConfigurationProvider)
                .AsEnumerable()
                // 进行数据塑形和分页返回
                .ShapeData(_shaper, request.Fields)
                .PaginatedListFromEnumerable(request.PageNumber, request.PageSize)
            );
    }
}