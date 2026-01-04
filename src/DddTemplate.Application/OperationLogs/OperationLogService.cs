using DddTemplate.Domain.OperationLogs;
using Microsoft.Extensions.Logging;

namespace DddTemplate.Application.OperationLogs;

public sealed class OperationLogService
{
    private readonly IOperationLogRepository _repository;
    private readonly ILogger<OperationLogService> _logger;

    public OperationLogService(IOperationLogRepository repository, ILogger<OperationLogService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<OperationLogDto> CreateAsync(CreateOperationLogRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Creating operation log for user: {UserName}, module: {Module}", request.UserName, request.Module);

        var entity = OperationLog.Create(
            request.UserName,
            request.Module,
            request.OperationType,
            request.Description,
            request.RequestPath,
            request.RequestMethod,
            request.RequestParams,
            request.IpAddress,
            request.IsSuccess,
            request.ErrorMessage,
            request.ExecutionTime);

        await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Operation log created with ID: {LogId}", entity.Id);
        return ToDto(entity);
    }

    public async Task<IReadOnlyList<OperationLogDto>> ListAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving all operation logs");

        var entities = await _repository.ListAsync(ct);

        _logger.LogDebug("Retrieved {Count} operation logs", entities.Count);
        return entities.Select(ToDto).ToList();
    }

    public async Task<OperationLogDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving operation log with ID: {LogId}", id);

        var entity = await _repository.GetByIdAsync(id, ct);

        if (entity is null)
        {
            _logger.LogWarning("Operation log with ID {LogId} not found", id);
            return null;
        }

        return ToDto(entity);
    }

    public async Task<IReadOnlyList<OperationLogDto>> GetByModuleAsync(string module, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving operation logs for module: {Module}", module);

        var entities = await _repository.GetByModuleAsync(module, ct);

        return entities.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<OperationLogDto>> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving operation logs for user: {UserName}", userName);

        var entities = await _repository.GetByUserNameAsync(userName, ct);

        return entities.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<OperationLogDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving operation logs from {StartDate} to {EndDate}", startDate, endDate);

        var entities = await _repository.GetByDateRangeAsync(startDate, endDate, ct);

        return entities.Select(ToDto).ToList();
    }

    private static OperationLogDto ToDto(OperationLog entity) =>
        new(
            entity.Id,
            entity.UserName,
            entity.Module,
            entity.OperationType,
            entity.Description,
            entity.RequestPath,
            entity.RequestMethod,
            entity.RequestParams,
            entity.IpAddress,
            entity.IsSuccess,
            entity.ErrorMessage,
            entity.OperatedAt,
            entity.ExecutionTime);
}
