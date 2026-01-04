namespace DddTemplate.Admin.Models;

/// <summary>
/// 待办事项数据传输对象
/// 用于在前端和后端之间传递待办事项数据
/// </summary>
public class TodoItemDto
{
    /// <summary>
    /// 待办事项 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 待办事项标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项描述（可选）
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 完成时间（可选）
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// 创建待办事项请求
/// </summary>
public class CreateTodoItemRequest
{
    /// <summary>
    /// 待办事项标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项描述（可选）
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 更新待办事项请求
/// </summary>
public class UpdateTodoItemRequest
{
    /// <summary>
    /// 待办事项标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 待办事项描述（可选）
    /// </summary>
    public string? Description { get; set; }
}
