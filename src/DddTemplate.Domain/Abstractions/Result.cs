namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 表示操作结果的基类
/// 用于替代异常处理，提供更优雅的错误处理方式
/// </summary>
public class Result
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 操作是否失败
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// 错误信息
    /// </summary>
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// 创建失败结果
    /// </summary>
    /// <param name="error">错误信息</param>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// 创建泛型成功结果
    /// </summary>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="value">值</param>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// 创建泛型失败结果
    /// </summary>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="error">错误信息</param>
    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);

    /// <summary>
    /// 从值创建结果
    /// </summary>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="value">值</param>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

/// <summary>
/// 表示带有返回值的操作结果
/// </summary>
/// <typeparam name="TValue">返回值类型</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// 操作结果值
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// 转换为非泛型 Result
    /// </summary>
    public Result ToResult() =>
        IsSuccess ? Success() : Failure(Error);
}
