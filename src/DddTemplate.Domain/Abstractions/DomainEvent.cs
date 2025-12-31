namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 领域事件基类
/// 提供领域事件的默认实现
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// 事件发生的时间
    /// </summary>
    public DateTime OccurredOn { get; init; }

    /// <summary>
    /// 事件唯一标识
    /// </summary>
    public Guid EventId { get; init; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
