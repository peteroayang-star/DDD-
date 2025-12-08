namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 领域异常基类
/// 用于表示领域层的业务规则违反
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public Error Error { get; }

    protected DomainException(Error error)
        : base(error.Message)
    {
        Error = error;
    }

    protected DomainException(Error error, Exception innerException)
        : base(error.Message, innerException)
    {
        Error = error;
    }
}

/// <summary>
/// 验证异常
/// 当输入数据不符合验证规则时抛出
/// </summary>
public sealed class ValidationException : DomainException
{
    public ValidationException(Error error)
        : base(error)
    {
    }

    public ValidationException(string code, string message)
        : base(Error.Validation(code, message))
    {
    }
}

/// <summary>
/// 未找到异常
/// 当请求的资源不存在时抛出
/// </summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(Error error)
        : base(error)
    {
    }

    public NotFoundException(string code, string message)
        : base(Error.NotFound(code, message))
    {
    }
}

/// <summary>
/// 冲突异常
/// 当操作与当前状态冲突时抛出
/// </summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(Error error)
        : base(error)
    {
    }

    public ConflictException(string code, string message)
        : base(Error.Conflict(code, message))
    {
    }
}

/// <summary>
/// 业务规则异常
/// 当违反业务规则时抛出
/// </summary>
public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException(Error error)
        : base(error)
    {
    }

    public BusinessRuleException(string code, string message)
        : base(Error.Failure(code, message))
    {
    }
}
