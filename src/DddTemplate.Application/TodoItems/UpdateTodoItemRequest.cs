namespace DddTemplate.Application.TodoItems;

public sealed record UpdateTodoItemRequest(
    string Title,
    string? Description
);
