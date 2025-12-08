using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.TodoItems.Events;

/// <summary>
/// TodoItem 完成事件
/// 当 TodoItem 被标记为完成时触发
/// </summary>
public sealed record TodoItemCompletedEvent : DomainEvent
{
    /// <summary>
    /// TodoItem ID
    /// </summary>
    public Guid TodoItemId { get; init; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime CompletedAt { get; init; }

    public TodoItemCompletedEvent(Guid todoItemId)
    {
        TodoItemId = todoItemId;
        CompletedAt = DateTime.UtcNow;
    }
}
