using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;
using Microsoft.Extensions.Logging;

namespace DddTemplate.Application.TodoItems;

public sealed class TodoItemService
{
    private readonly ITodoItemRepository _repository;
    private readonly ILogger<TodoItemService> _logger;

    public TodoItemService(ITodoItemRepository repository, ILogger<TodoItemService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating todo item with title: {Title}", request.Title);

        var entity = TodoItem.Create(request.Title);
        await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Todo item created successfully with ID: {TodoId}", entity.Id);
        return ToDto(entity);
    }

    public async Task<IReadOnlyList<TodoItemDto>> ListAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving all todo items from repository");

        var entities = await _repository.ListAsync(ct);

        _logger.LogDebug("Retrieved {Count} todo items from repository", entities.Count);
        return entities.Select(ToDto).ToList();
    }

    public async Task<TodoItemDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving todo item with ID: {TodoId}", id);

        var entity = await _repository.GetByIdAsync(id, ct);

        if (entity is null)
        {
            _logger.LogWarning("Todo item with ID {TodoId} not found in repository", id);
            return null;
        }

        _logger.LogDebug("Todo item {TodoId} retrieved successfully", id);
        return ToDto(entity);
    }

    public async Task<bool> CompleteAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Attempting to complete todo item: {TodoId}", id);

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            _logger.LogWarning("Cannot complete todo item {TodoId} - not found", id);
            return false;
        }

        entity.MarkCompleted();
        // InMemory 这里不需要额外 save，别的实现可以在仓储里处理

        _logger.LogInformation("Todo item {TodoId} completed successfully", id);
        return true;
    }

    public async Task<bool> RenameAsync(Guid id, string newTitle, CancellationToken ct = default)
    {
        _logger.LogInformation("Attempting to rename todo item {TodoId} to: {NewTitle}", id, newTitle);

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            _logger.LogWarning("Cannot rename todo item {TodoId} - not found", id);
            return false;
        }

        var oldTitle = entity.Title;
        entity.Rename(newTitle);

        _logger.LogInformation("Todo item {TodoId} renamed from '{OldTitle}' to '{NewTitle}'", id, oldTitle, newTitle);
        return true;
    }

    private static TodoItemDto ToDto(TodoItem entity) =>
        new(entity.Id, entity.Title, entity.IsCompleted, entity.CreatedAt);
}

