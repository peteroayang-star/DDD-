using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Users.Events;

/// <summary>
/// 用户激活事件
/// </summary>
public sealed record UserActivatedEvent : DomainEvent
{
    public Guid UserId { get; init; }

    public UserActivatedEvent(Guid userId)
    {
        UserId = userId;
    }
}
