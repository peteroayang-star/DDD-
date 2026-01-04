using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Menus;

/// <summary>
/// 菜单聚合根
/// 表示系统中的菜单项
/// </summary>
public sealed class Menu : AggregateRoot<Guid>
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// 菜单路径
    /// </summary>
    public string? Path { get; private set; }

    /// <summary>
    /// 父菜单ID（null表示顶级菜单）
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private Menu() : base(Guid.Empty)
    {
        Name = string.Empty;
    }

    private Menu(Guid id, string name, string? icon, string? path, Guid? parentId, int sortOrder)
        : base(id)
    {
        Name = name;
        Icon = icon;
        Path = path;
        ParentId = parentId;
        SortOrder = sortOrder;
        IsEnabled = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 创建新菜单
    /// </summary>
    public static Menu Create(string name, string? icon = null, string? path = null, Guid? parentId = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu name cannot be empty.", nameof(name));

        return new Menu(Guid.NewGuid(), name.Trim(), icon?.Trim(), path?.Trim(), parentId, sortOrder);
    }

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    public void Update(string name, string? icon, string? path, Guid? parentId, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu name cannot be empty.", nameof(name));

        Name = name.Trim();
        Icon = icon?.Trim();
        Path = path?.Trim();
        ParentId = parentId;
        SortOrder = sortOrder;
    }

    /// <summary>
    /// 启用菜单
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
    }

    /// <summary>
    /// 禁用菜单
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
    }
}
