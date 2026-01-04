using DddTemplate.Domain.Menus;
using Microsoft.Extensions.Logging;

namespace DddTemplate.Application.Menus;

public sealed class MenuService
{
    private readonly IMenuRepository _repository;
    private readonly ILogger<MenuService> _logger;

    public MenuService(IMenuRepository repository, ILogger<MenuService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<MenuDto> CreateAsync(CreateMenuRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating menu with name: {Name}", request.Name);

        var entity = Menu.Create(request.Name, request.Icon, request.Path, request.ParentId, request.SortOrder);
        await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Menu created successfully with ID: {MenuId}", entity.Id);
        return ToDto(entity);
    }

    public async Task<IReadOnlyList<MenuDto>> ListAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving all menus");
        var entities = await _repository.ListAsync(ct);
        return entities.Select(ToDto).ToList();
    }

    public async Task<MenuDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Retrieving menu with ID: {MenuId}", id);
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity == null ? null : ToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMenuRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating menu {MenuId}", id);
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.Update(request.Name, request.Icon, request.Path, request.ParentId, request.SortOrder);
        _logger.LogInformation("Menu {MenuId} updated successfully", id);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting menu {MenuId}", id);
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        await _repository.RemoveAsync(entity, ct);
        _logger.LogInformation("Menu {MenuId} deleted successfully", id);
        return true;
    }

    private static MenuDto ToDto(Menu entity) =>
        new(entity.Id, entity.Name, entity.Icon, entity.Path, entity.ParentId, entity.SortOrder, entity.IsEnabled, entity.CreatedAt);
}
