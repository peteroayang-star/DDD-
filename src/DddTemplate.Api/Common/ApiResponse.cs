namespace DddTemplate.Api.Common;

/// <summary>
/// 统一的 API 响应格式
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public ErrorDetails? Error { get; init; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    private ApiResponse(bool success, T? data, ErrorDetails? error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data) =>
        new(true, data, null);

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse<T> FailureResponse(ErrorDetails error) =>
        new(false, default, error);
}

/// <summary>
/// 无数据的 API 响应格式
/// </summary>
public sealed class ApiResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public ErrorDetails? Error { get; init; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    private ApiResponse(bool success, string? message, ErrorDetails? error)
    {
        Success = success;
        Message = message;
        Error = error;
    }

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse SuccessResponse(string? message = null) =>
        new(true, message, null);

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse FailureResponse(ErrorDetails error) =>
        new(false, null, error);
}

/// <summary>
/// 错误详情
/// </summary>
public sealed class ErrorDetails
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// 错误类型
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// 详细错误信息（仅开发环境）
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// 验证错误集合
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public ErrorDetails(string code, string message, string type, string? details = null, Dictionary<string, string[]>? validationErrors = null)
    {
        Code = code;
        Message = message;
        Type = type;
        Details = details;
        ValidationErrors = validationErrors;
    }
}
