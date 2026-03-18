using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Menus.Events;

public sealed record MenuCreatedEvent(Guid MenuId, string Name) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}

public sealed record MenuUpdatedEvent(Guid MenuId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}

public sealed record MenuDisabledEvent(Guid MenuId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}
