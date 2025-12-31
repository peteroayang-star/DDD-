using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.Abstractions.Messaging;

/// <summary>
/// 表示一个命令（写操作）
/// 命令用于修改系统状态
/// </summary>
public interface ICommand
{
}

/// <summary>
/// 表示一个带返回值的命令
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface ICommand<out TResponse> : ICommand
{
}
