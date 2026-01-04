using DddTemplate.Domain.OperationLogs;
using DddTemplate.Infrastructure.InMemory.Common;

namespace DddTemplate.Infrastructure.InMemory.OperationLogs;

/// <summary>
/// 操作日志内存仓储实现
/// </summary>
public sealed class OperationLogRepository : InMemoryRepository<OperationLog, Guid>, IOperationLogRepository
{
    public Task<IReadOnlyList<OperationLog>> GetByModuleAsync(string module, CancellationToken ct = default)
    {
        var logs = Store.Values
            .Where(log => log.Module.Equals(module, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(log => log.OperatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<OperationLog>>(logs);
    }

    public Task<IReadOnlyList<OperationLog>> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        var logs = Store.Values
            .Where(log => log.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(log => log.OperatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<OperationLog>>(logs);
    }

    public Task<IReadOnlyList<OperationLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var logs = Store.Values
            .Where(log => log.OperatedAt >= startDate && log.OperatedAt <= endDate)
            .OrderByDescending(log => log.OperatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<OperationLog>>(logs);
    }
}
