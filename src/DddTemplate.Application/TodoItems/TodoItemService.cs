using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems;

public sealed class TodoItemService
{
    private readonly ITodoItemRepository _repository;

    public TodoItemService(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemRequest request, CancellationToken ct = default)
    {
        var entity = TodoItem.Create(request.Title);
        await _repository.AddAsync(entity, ct);
        return ToDto(entity);
    }

    public async Task<IReadOnlyList<TodoItemDto>> ListAsync(CancellationToken ct = default)
    {
        var entities = await _repository.ListAsync(ct);
        return entities.Select(ToDto).ToList();
    }

    public async Task<TodoItemDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<bool> CompleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.MarkCompleted();
        // InMemory 这里不需要额外 save，别的实现可以在仓储里处理
        return true;
    }

    public async Task<bool> RenameAsync(Guid id, string newTitle, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.Rename(newTitle);
        return true;
    }

    private static TodoItemDto ToDto(TodoItem entity) =>
        new(entity.Id, entity.Title, entity.IsCompleted, entity.CreatedAt);
}

