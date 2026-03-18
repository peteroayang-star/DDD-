using DddTemplate.Application.Abstractions;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems.Queries;

public sealed class GetAllTodoItemsQueryHandler : IQueryHandler<GetAllTodoItemsQuery, Result<List<TodoItemDto>>>
{
    private readonly ITodoItemRepository _repository;

    public GetAllTodoItemsQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<TodoItemDto>>> Handle(GetAllTodoItemsQuery request, CancellationToken cancellationToken)
    {
        var todoItems = await _repository.ListAsync(cancellationToken);

        var dtos = todoItems.Select(t => new TodoItemDto(
            t.Id,
            t.Title,
            t.Description,
            t.IsCompleted,
            t.CreatedAt
        )).ToList();

        return Result.Success(dtos);
    }
}
