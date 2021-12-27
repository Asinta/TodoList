using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Models;
using TodoList.Application.TodoLists.Commands.CreateTodoList;
using TodoList.Application.TodoLists.Queries.GetSingleTodo;
using TodoList.Application.TodoLists.Queries.GetTodos;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("/todo-list")]
public class TodoListController : ControllerBase
{
    private readonly IMediator _mediator;

    // 注入MediatR
    public TodoListController(IMediator mediator) 
        => _mediator = mediator;
    
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
    
    [HttpPost]
    public async Task<ApiResponse<Domain.Entities.TodoList>> Create([FromBody] CreateTodoListCommand command)
    {
        return ApiResponse<Domain.Entities.TodoList>.Success(await _mediator.Send(command));
    }
}