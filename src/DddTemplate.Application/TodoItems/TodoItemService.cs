using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;
using DddTemplate.Application.OperationLogs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace DddTemplate.Application.TodoItems;

public sealed class TodoItemService
{
    private readonly ITodoItemRepository _repository;
    private readonly OperationLogService _operationLogService;
    private readonly ILogger<TodoItemService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TodoItemService(
        ITodoItemRepository repository,
        OperationLogService operationLogService,
        ILogger<TodoItemService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _operationLogService = operationLogService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // 尝试从 X-Forwarded-For 获取
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // 从连接信息获取
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            return remoteIp.ToString() == "::1" ? "127.0.0.1" : remoteIp.ToString();
        }

        return null;
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemRequest request, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Creating todo item with title: {Title}", request.Title);

        try
        {
            var entity = TodoItem.Create(request.Title, request.Description);
            await _repository.AddAsync(entity, ct);

            var executionTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            // 记录操作日志
            await _operationLogService.CreateAsync(new CreateOperationLogRequest(
                UserName: "System",
                Module: "TodoItem",
                OperationType: "Create",
                Description: $"创建待办事项: {request.Title}",
                RequestPath: "/api/todos",
                RequestMethod: "POST",
                RequestParams: System.Text.Json.JsonSerializer.Serialize(request),
                IpAddress: GetClientIpAddress(),
                IsSuccess: true,
                ErrorMessage: null,
                ExecutionTime: executionTime
            ), ct);

            _logger.LogInformation("Todo item created successfully with ID: {TodoId}", entity.Id);
            return ToDto(entity);
        }
        catch (Exception ex)
        {
            var executionTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            // 记录失败日志
            await _operationLogService.CreateAsync(new CreateOperationLogRequest(
                UserName: "System",
                Module: "TodoItem",
                OperationType: "Create",
                Description: $"创建待办事项失败: {request.Title}",
                RequestPath: "/api/todos",
                RequestMethod: "POST",
                RequestParams: System.Text.Json.JsonSerializer.Serialize(request),
                IpAddress: GetClientIpAddress(),
                IsSuccess: false,
                ErrorMessage: ex.Message,
                ExecutionTime: executionTime
            ), ct);

            throw;
        }
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
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Attempting to complete todo item: {TodoId}", id);

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            _logger.LogWarning("Cannot complete todo item {TodoId} - not found", id);
            return false;
        }

        entity.MarkCompleted();
        var executionTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

        // 记录操作日志
        await _operationLogService.CreateAsync(new CreateOperationLogRequest(
            UserName: "System",
            Module: "TodoItem",
            OperationType: "Update",
            Description: $"完成待办事项: {entity.Title}",
            RequestPath: $"/api/todos/{id}/complete",
            RequestMethod: "PUT",
            RequestParams: null,
            IpAddress: GetClientIpAddress(),
            IsSuccess: true,
            ErrorMessage: null,
            ExecutionTime: executionTime
        ), ct);

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

    public async Task<bool> UpdateAsync(Guid id, UpdateTodoItemRequest request, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Attempting to update todo item {TodoId}", id);

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            _logger.LogWarning("Cannot update todo item {TodoId} - not found", id);
            return false;
        }

        entity.Rename(request.Title);
        entity.UpdateDescription(request.Description);
        var executionTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

        // 记录操作日志
        await _operationLogService.CreateAsync(new CreateOperationLogRequest(
            UserName: "System",
            Module: "TodoItem",
            OperationType: "Update",
            Description: $"更新待办事项: {request.Title}",
            RequestPath: $"/api/todos/{id}",
            RequestMethod: "PUT",
            RequestParams: System.Text.Json.JsonSerializer.Serialize(request),
            IpAddress: GetClientIpAddress(),
            IsSuccess: true,
            ErrorMessage: null,
            ExecutionTime: executionTime
        ), ct);

        _logger.LogInformation("Todo item {TodoId} updated successfully", id);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Attempting to delete todo item {TodoId}", id);

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
        {
            _logger.LogWarning("Cannot delete todo item {TodoId} - not found", id);
            return false;
        }

        var title = entity.Title;
        await _repository.RemoveAsync(entity, ct);
        var executionTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

        // 记录操作日志
        await _operationLogService.CreateAsync(new CreateOperationLogRequest(
            UserName: "System",
            Module: "TodoItem",
            OperationType: "Delete",
            Description: $"删除待办事项: {title}",
            RequestPath: $"/api/todos/{id}",
            RequestMethod: "DELETE",
            RequestParams: null,
            IpAddress: GetClientIpAddress(),
            IsSuccess: true,
            ErrorMessage: null,
            ExecutionTime: executionTime
        ), ct);

        _logger.LogInformation("Todo item {TodoId} deleted successfully", id);
        return true;
    }

    private static TodoItemDto ToDto(TodoItem entity) =>
        new(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt);
}

