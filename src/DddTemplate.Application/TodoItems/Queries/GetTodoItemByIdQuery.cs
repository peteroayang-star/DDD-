using DddTemplate.Application.Abstractions;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.TodoItems.Queries;

public sealed record GetTodoItemByIdQuery(Guid Id) : IQuery<Result<TodoItemDto>>;
