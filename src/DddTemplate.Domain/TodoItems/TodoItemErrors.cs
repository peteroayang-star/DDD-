using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.TodoItems;

public static class TodoItemErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "TodoItem.NotFound",
        "TodoItem not found");
}
