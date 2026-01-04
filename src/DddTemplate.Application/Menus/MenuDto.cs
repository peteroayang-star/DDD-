namespace DddTemplate.Application.Menus;

/// <summary>
/// 菜单数据传输对象
/// </summary>
public sealed record MenuDto(
    Guid Id,
    string Name,
    string? Icon,
    string? Path,
    Guid? ParentId,
    int SortOrder,
    bool IsEnabled,
    DateTime CreatedAt
);
