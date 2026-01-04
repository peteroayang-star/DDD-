using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.OperationLogs;

/// <summary>
/// 操作日志聚合根
/// 记录系统中的所有操作行为
/// </summary>
public sealed class OperationLog : AggregateRoot<Guid>
{
    /// <summary>
    /// 操作用户
    /// </summary>
    public string UserName { get; private set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    public string Module { get; private set; }

    /// <summary>
    /// 操作类型（创建、更新、删除、查询等）
    /// </summary>
    public string OperationType { get; private set; }

    /// <summary>
    /// 操作描述
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string? RequestPath { get; private set; }

    /// <summary>
    /// 请求方法（GET, POST, PUT, DELETE等）
    /// </summary>
    public string? RequestMethod { get; private set; }

    /// <summary>
    /// 请求参数（JSON格式）
    /// </summary>
    public string? RequestParams { get; private set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// 操作结果（成功/失败）
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// 错误信息（如果失败）
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime OperatedAt { get; private set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long ExecutionTime { get; private set; }

    private OperationLog() : base(Guid.Empty)
    {
        UserName = string.Empty;
        Module = string.Empty;
        OperationType = string.Empty;
        Description = string.Empty;
    }

    private OperationLog(
        Guid id,
        string userName,
        string module,
        string operationType,
        string description,
        string? requestPath,
        string? requestMethod,
        string? requestParams,
        string? ipAddress,
        bool isSuccess,
        string? errorMessage,
        long executionTime)
        : base(id)
    {
        UserName = userName;
        Module = module;
        OperationType = operationType;
        Description = description;
        RequestPath = requestPath;
        RequestMethod = requestMethod;
        RequestParams = requestParams;
        IpAddress = ipAddress;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ExecutionTime = executionTime;
        OperatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 创建操作日志
    /// </summary>
    public static OperationLog Create(
        string userName,
        string module,
        string operationType,
        string description,
        string? requestPath = null,
        string? requestMethod = null,
        string? requestParams = null,
        string? ipAddress = null,
        bool isSuccess = true,
        string? errorMessage = null,
        long executionTime = 0)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName cannot be empty.", nameof(userName));

        if (string.IsNullOrWhiteSpace(module))
            throw new ArgumentException("Module cannot be empty.", nameof(module));

        if (string.IsNullOrWhiteSpace(operationType))
            throw new ArgumentException("OperationType cannot be empty.", nameof(operationType));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        return new OperationLog(
            Guid.NewGuid(),
            userName.Trim(),
            module.Trim(),
            operationType.Trim(),
            description.Trim(),
            requestPath?.Trim(),
            requestMethod?.Trim(),
            requestParams,
            ipAddress?.Trim(),
            isSuccess,
            errorMessage?.Trim(),
            executionTime);
    }
}
