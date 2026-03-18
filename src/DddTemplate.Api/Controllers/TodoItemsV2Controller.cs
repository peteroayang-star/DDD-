using MediatR;
using Microsoft.AspNetCore.Mvc;
using DddTemplate.Application.TodoItems.Commands;
using DddTemplate.Application.TodoItems.Queries;
using DddTemplate.Api.Common;

namespace DddTemplate.Api.Controllers;

[ApiController]
[Route("api/v2/[controller]")]
public class TodoItemsV2Controller : ControllerBase
{
    private readonly ISender _sender;

    public TodoItemsV2Controller(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var query = new GetAllTodoItemsQuery();
        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<List<Application.TodoItems.TodoItemDto>>.SuccessResponse(result.Value))
            : NotFound(ApiResponse<List<Application.TodoItems.TodoItemDto>>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetTodoItemByIdQuery(id);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<Application.TodoItems.TodoItemDto>.SuccessResponse(result.Value))
            : NotFound(ApiResponse<Application.TodoItems.TodoItemDto>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoItemCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value },
                ApiResponse<Guid>.SuccessResponse(result.Value))
            : BadRequest(ApiResponse<Guid>.FailureResponse(
                new ErrorDetails(result.Error.Code, result.Error.Message, result.Error.Type.ToString())));
    }
}
