using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.OperationLogs;

/// <summary>
/// 操作日志仓储接口
/// </summary>
public interface IOperationLogRepository : IRepository<OperationLog, Guid>
{
    /// <summary>
    /// 根据模块查询操作日志
    /// </summary>
    Task<IReadOnlyList<OperationLog>> GetByModuleAsync(string module, CancellationToken ct = default);

    /// <summary>
    /// 根据用户名查询操作日志
    /// </summary>
    Task<IReadOnlyList<OperationLog>> GetByUserNameAsync(string userName, CancellationToken ct = default);

    /// <summary>
    /// 根据时间范围查询操作日志
    /// </summary>
    Task<IReadOnlyList<OperationLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
