using DddTemplate.Application.Abstractions;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed record CreateTodoItemCommand(string Title, string? Description) : ICommand<Result<Guid>>;
