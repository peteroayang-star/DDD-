using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.Abstractions.Messaging;

/// <summary>
/// 命令处理器接口
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// 处理命令
    /// </summary>
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// 无返回值的命令处理器接口
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// 处理命令
    /// </summary>
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
