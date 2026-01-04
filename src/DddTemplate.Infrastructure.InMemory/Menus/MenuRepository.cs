using DddTemplate.Domain.Menus;
using DddTemplate.Infrastructure.InMemory.Common;

namespace DddTemplate.Infrastructure.InMemory.Menus;

/// <summary>
/// 菜单内存仓储实现
/// </summary>
public sealed class MenuRepository : InMemoryRepository<Menu, Guid>, IMenuRepository
{
    public Task<IReadOnlyList<Menu>> GetTopLevelMenusAsync(CancellationToken ct = default)
    {
        var menus = Store.Values
            .Where(m => m.ParentId == null)
            .OrderBy(m => m.SortOrder)
            .ToList();

        return Task.FromResult<IReadOnlyList<Menu>>(menus);
    }

    public Task<IReadOnlyList<Menu>> GetByParentIdAsync(Guid parentId, CancellationToken ct = default)
    {
        var menus = Store.Values
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.SortOrder)
            .ToList();

        return Task.FromResult<IReadOnlyList<Menu>>(menus);
    }
}
