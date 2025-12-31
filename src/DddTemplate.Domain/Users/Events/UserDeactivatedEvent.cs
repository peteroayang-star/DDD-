using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Users.Events;

/// <summary>
/// 用户停用事件
/// </summary>
public sealed record UserDeactivatedEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string Reason { get; init; }

    public UserDeactivatedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
    }
}
