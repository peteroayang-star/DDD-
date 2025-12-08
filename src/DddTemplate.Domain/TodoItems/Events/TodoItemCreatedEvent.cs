using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.TodoItems.Events;

/// <summary>
/// TodoItem 创建事件
/// 当新的 TodoItem 被创建时触发
/// </summary>
public sealed record TodoItemCreatedEvent : DomainEvent
{
    /// <summary>
    /// TodoItem ID
    /// </summary>
    public Guid TodoItemId { get; init; }

    /// <summary>
    /// TodoItem 标题
    /// </summary>
    public string Title { get; init; }

    public TodoItemCreatedEvent(Guid todoItemId, string title)
    {
        TodoItemId = todoItemId;
        Title = title;
    }
}
