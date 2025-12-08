using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.TodoItems.Events;

/// <summary>
/// TodoItem 重命名事件
/// 当 TodoItem 的标题被修改时触发
/// </summary>
public sealed record TodoItemRenamedEvent : DomainEvent
{
    /// <summary>
    /// TodoItem ID
    /// </summary>
    public Guid TodoItemId { get; init; }

    /// <summary>
    /// 旧标题
    /// </summary>
    public string OldTitle { get; init; }

    /// <summary>
    /// 新标题
    /// </summary>
    public string NewTitle { get; init; }

    public TodoItemRenamedEvent(Guid todoItemId, string oldTitle, string newTitle)
    {
        TodoItemId = todoItemId;
        OldTitle = oldTitle;
        NewTitle = newTitle;
    }
}
