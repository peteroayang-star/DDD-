namespace DddTemplate.Domain.Abstractions;

/// <summary>
/// 领域事件接口
/// 领域事件表示领域中发生的重要业务事件
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 事件发生的时间
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// 事件唯一标识
    /// </summary>
    Guid EventId { get; }
}
