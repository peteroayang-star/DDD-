using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Users.Events;

/// <summary>
/// 用户创建事件
/// </summary>
public sealed record UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string FullName { get; init; }

    public UserCreatedEvent(Guid userId, string email, string fullName)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
    }
}
