namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 表示错误信息
/// </summary>
public sealed record Error
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 错误类型
    /// </summary>
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// 表示没有错误
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>
    /// 表示空值错误
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null.", ErrorType.Validation);

    /// <summary>
    /// 创建验证错误
    /// </summary>
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// 创建未找到错误
    /// </summary>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    /// <summary>
    /// 创建冲突错误
    /// </summary>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>
    /// 创建业务规则错误
    /// </summary>
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    /// <summary>
    /// 创建未授权错误
    /// </summary>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// 创建禁止访问错误
    /// </summary>
    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);
}

/// <summary>
/// 错误类型枚举
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// 无错误
    /// </summary>
    None = 0,

    /// <summary>
    /// 验证错误
    /// </summary>
    Validation = 1,

    /// <summary>
    /// 未找到
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// 冲突
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// 业务规则失败
    /// </summary>
    Failure = 4,

    /// <summary>
    /// 未授权
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// 禁止访问
    /// </summary>
    Forbidden = 6
}
