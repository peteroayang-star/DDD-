namespace DddTemplate.Application.Menus;

/// <summary>
/// 创建菜单请求
/// </summary>
public sealed record CreateMenuRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Path { get; init; }
    public Guid? ParentId { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// 更新菜单请求
/// </summary>
public sealed record UpdateMenuRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public string? Path { get; init; }
    public Guid? ParentId { get; init; }
    public int SortOrder { get; init; }
}
