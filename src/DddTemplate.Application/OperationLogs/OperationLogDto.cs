namespace DddTemplate.Application.OperationLogs;

/// <summary>
/// 操作日志数据传输对象
/// </summary>
public sealed record OperationLogDto(
    Guid Id,
    string UserName,
    string Module,
    string OperationType,
    string Description,
    string? RequestPath,
    string? RequestMethod,
    string? RequestParams,
    string? IpAddress,
    bool IsSuccess,
    string? ErrorMessage,
    DateTime OperatedAt,
    long ExecutionTime
);
