using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Models;
using TodoList.Application.TodoItems.Commands.CreateTodoItem;
using TodoList.Application.TodoItems.Commands.PatchTodoItem;
using TodoList.Application.TodoItems.Commands.UpdateTodoItem;
using TodoList.Application.TodoItems.Queries.GetTodoItems;
using TodoList.Domain.Entities;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("/todo-item")]
public class TodoItemController : ControllerBase
{
    private readonly IMediator _mediator;

    // 注入MediatR
    public TodoItemController(IMediator mediator) 
        => _mediator = mediator;
    
    [HttpPost]
    public async Task<ApiResponse<TodoItem>> Create([FromBody] CreateTodoItemCommand command)
    {
        return ApiResponse<TodoItem>.Success(await _mediator.Send(command));
    }
    
    [HttpPut("{id:Guid}")]
    public async Task<ApiResponse<TodoItem>> Update(Guid id, [FromBody] UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return ApiResponse<TodoItem>.Fail("Query id not match witch body");
        }
        
        return ApiResponse<TodoItem>.Success(await _mediator.Send(command));
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<ApiResponse<TodoItem>> Patch(Guid id, [FromBody] JsonPatchDocument<TestPatchTodo> command)
    {
        if (command is null)
        {
            return ApiResponse<TodoItem>.Fail("patch document should not be null");
        }
        
        // if (id != command.Id)
        // {
            // return ApiResponse<TodoItem>.Fail("Query id not match witch body");
        // }
        return ApiResponse<TodoItem>.Fail("BINGO");
        
        
        // return ApiResponse<TodoItem>.Success(await _mediator.Send(command));
    }
}