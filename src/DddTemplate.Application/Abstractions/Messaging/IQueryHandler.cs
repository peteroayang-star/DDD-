using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.Abstractions.Messaging;

/// <summary>
/// 查询处理器接口
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// 处理查询
    /// </summary>
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
