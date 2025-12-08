namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 聚合根基类
/// 聚合根是一组相关对象的集合，作为数据修改的单元
/// 聚合根负责维护聚合内的一致性边界
/// </summary>
/// <typeparam name="TId">聚合根标识类型</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// 获取领域事件集合（只读）
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() : base() { }

    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// 清除所有领域事件
    /// 通常在事件发布后调用
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// 移除指定的领域事件
    /// </summary>
    /// <param name="domainEvent">要移除的领域事件</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
}
