using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems.Events;

namespace DddTemplate.Domain.TodoItems;

/// <summary>
/// TodoItem 聚合根
/// 表示一个待办事项
/// </summary>
public sealed class TodoItem : AggregateRoot<Guid>
{
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TodoItem() : base(Guid.Empty)
    {
        Title = string.Empty; // for serialization
    }

    private TodoItem(Guid id, string title)
        : base(id)
    {
        Title = title;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 创建新的 TodoItem
    /// </summary>
    /// <param name="title">标题</param>
    /// <returns>TodoItem 实例</returns>
    public static TodoItem Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        var todoItem = new TodoItem(Guid.NewGuid(), title.Trim());

        // 触发领域事件
        todoItem.AddDomainEvent(new TodoItemCreatedEvent(todoItem.Id, todoItem.Title));

        return todoItem;
    }

    /// <summary>
    /// 标记为完成
    /// </summary>
    public void MarkCompleted()
    {
        if (IsCompleted) return; // 幂等操作

        IsCompleted = true;

        // 触发领域事件
        AddDomainEvent(new TodoItemCompletedEvent(Id));
    }

    /// <summary>
    /// 重命名
    /// </summary>
    /// <param name="newTitle">新标题</param>
    public void Rename(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty.", nameof(newTitle));

        var oldTitle = Title;
        Title = newTitle.Trim();

        // 触发领域事件
        AddDomainEvent(new TodoItemRenamedEvent(Id, oldTitle, Title));
    }
}

