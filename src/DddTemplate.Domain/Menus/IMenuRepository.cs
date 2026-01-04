using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.Menus;

/// <summary>
/// 菜单仓储接口
/// </summary>
public interface IMenuRepository : IRepository<Menu, Guid>
{
    /// <summary>
    /// 获取所有顶级菜单
    /// </summary>
    Task<IReadOnlyList<Menu>> GetTopLevelMenusAsync(CancellationToken ct = default);

    /// <summary>
    /// 根据父菜单ID获取子菜单
    /// </summary>
    Task<IReadOnlyList<Menu>> GetByParentIdAsync(Guid parentId, CancellationToken ct = default);
}
