using DddTemplate.Application.Abstractions;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed class CreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, Result<Guid>>
{
    private readonly ITodoItemRepository _repository;

    public CreateTodoItemCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = TodoItem.Create(request.Title, request.Description);
        await _repository.AddAsync(todoItem);
        return Result.Success(todoItem.Id);
    }
}
