using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TodoList.Api.Filters;
using TodoList.Api.Models;
using TodoList.Application.TodoLists.Commands.CreateTodoList;
using TodoList.Application.TodoLists.Commands.DeleteTodoList;
using TodoList.Application.TodoLists.Queries.GetSingleTodo;
using TodoList.Application.TodoLists.Queries.GetTodos;

namespace TodoList.Api.Controllers;

[ApiController]
[Authorize]
[Route("/todo-list")]
public class TodoListController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<TodoListController> _localizer;

    // 注入MediatR
    public TodoListController(IMediator mediator, IStringLocalizer<TodoListController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet("meta")]
    public ApiResponse<string> GetTodoListMeta()
    {
        var response = ApiResponse<string>.Success(_localizer["TodoListMeta"]);
        return response;
    }

    [HttpGet]
    public async Task<ApiResponse<List<TodoListBriefDto>>> Get()
    {
        return ApiResponse<List<TodoListBriefDto>>.Success(await _mediator.Send(new GetTodosQuery()));
    }
    
    [HttpGet("{id:Guid}", Name = "TodListById")]
    public async Task<ApiResponse<TodoListDto>> GetSingleTodoList(Guid id)
    {
        return ApiResponse<TodoListDto>.Success(await _mediator.Send(new GetSingleTodoQuery { ListId = id }) ??
                                                throw new InvalidOperationException());
    }
    
    /// <summary>
    /// 创建新的TodoList，只有Administrator角色的用户有此权限
    /// </summary>
    /// <param name="command">创建TodoList命令</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "OnlyAdmin")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    public async Task<ApiResponse<Domain.Entities.TodoList>> Create([FromBody] CreateTodoListCommand command)
    {
        return ApiResponse<Domain.Entities.TodoList>.Success(await _mediator.Send(command));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ApiResponse<object>> Delete(Guid id)
    {
        return ApiResponse<object>.Success(await _mediator.Send(new DeleteTodoListCommand { Id = id }));
    }
}