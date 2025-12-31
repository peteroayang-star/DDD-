using Microsoft.AspNetCore.Mvc;
using DddTemplate.Application.TodoItems;
using DddTemplate.Api.Common;

namespace DddTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly TodoItemService _service;
    private readonly ILogger<TodoItemsController> _logger;

    public TodoItemsController(TodoItemService service, ILogger<TodoItemsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TodoItemDto>>>> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("Fetching all todo items");
        var items = await _service.ListAsync(ct);
        return Ok(ApiResponse<List<TodoItemDto>>.SuccessResponse(items.ToList()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TodoItemDto>>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Fetching todo item with ID: {TodoId}", id);
        var item = await _service.GetAsync(id, ct);

        if (item is null)
        {
            _logger.LogWarning("Todo item with ID {TodoId} not found", id);
            return NotFound(ApiResponse<TodoItemDto>.FailureResponse(
                new ErrorDetails("TodoItem.NotFound", "TodoItem not found", "NotFound")));
        }

        return Ok(ApiResponse<TodoItemDto>.SuccessResponse(item));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TodoItemDto>>> Create(CreateTodoItemRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating new todo item with title: {Title}", request.Title);
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<TodoItemDto>.SuccessResponse(created));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, UpdateTodoItemRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Updating todo item {TodoId}", id);
        var success = await _service.UpdateAsync(id, request, ct);

        if (!success)
        {
            _logger.LogWarning("Failed to update todo item {TodoId} - not found", id);
            return NotFound(ApiResponse<object>.FailureResponse(
                new ErrorDetails("TodoItem.NotFound", "TodoItem not found", "NotFound")));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Deleting todo item {TodoId}", id);
        var success = await _service.DeleteAsync(id, ct);

        if (!success)
        {
            _logger.LogWarning("Failed to delete todo item {TodoId} - not found", id);
            return NotFound(ApiResponse<object>.FailureResponse(
                new ErrorDetails("TodoItem.NotFound", "TodoItem not found", "NotFound")));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null));
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<ApiResponse<object>>> Complete(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Marking todo item {TodoId} as completed", id);
        var success = await _service.CompleteAsync(id, ct);

        if (!success)
        {
            _logger.LogWarning("Failed to complete todo item {TodoId} - not found", id);
            return NotFound(ApiResponse<object>.FailureResponse(
                new ErrorDetails("TodoItem.NotFound", "TodoItem not found", "NotFound")));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null));
    }
}
