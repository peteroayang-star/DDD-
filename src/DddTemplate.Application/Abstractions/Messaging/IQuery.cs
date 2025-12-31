namespace DddTemplate.Application.Abstractions.Messaging;

/// <summary>
/// 表示一个查询（读操作）
/// 查询不应修改系统状态
/// </summary>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface IQuery<out TResponse>
{
}
