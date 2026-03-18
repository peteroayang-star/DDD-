using DddTemplate.Application.Abstractions;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems.Queries;

public sealed class GetTodoItemByIdQueryHandler : IQueryHandler<GetTodoItemByIdQuery, Result<TodoItemDto>>
{
    private readonly ITodoItemRepository _repository;

    public GetTodoItemByIdQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TodoItemDto>> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<TodoItemDto>(TodoItemErrors.NotFound);
        }

        var dto = new TodoItemDto(
            todoItem.Id,
            todoItem.Title,
            todoItem.Description,
            todoItem.IsCompleted,
            todoItem.CreatedAt
        );

        return Result.Success(dto);
    }
}
