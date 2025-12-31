namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 聚合根标记接口
/// 聚合根是一组相关对象的集合，作为数据修改的单元
/// 只有聚合根可以被仓储直接访问
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// 获取领域事件集合
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// 添加领域事件
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// 清除所有领域事件
    /// </summary>
    void ClearDomainEvents();
}
