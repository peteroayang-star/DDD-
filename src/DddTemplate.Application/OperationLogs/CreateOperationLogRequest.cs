namespace DddTemplate.Application.OperationLogs;

/// <summary>
/// 创建操作日志请求
/// </summary>
public sealed record CreateOperationLogRequest(
    string UserName,
    string Module,
    string OperationType,
    string Description,
    string? RequestPath = null,
    string? RequestMethod = null,
    string? RequestParams = null,
    string? IpAddress = null,
    bool IsSuccess = true,
    string? ErrorMessage = null,
    long ExecutionTime = 0
);
