using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Users.Events;

/// <summary>
/// 用户邮箱变更事件
/// </summary>
public sealed record UserEmailChangedEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string OldEmail { get; init; }
    public string NewEmail { get; init; }

    public UserEmailChangedEvent(Guid userId, string oldEmail, string newEmail)
    {
        UserId = userId;
        OldEmail = oldEmail;
        NewEmail = newEmail;
    }
}
