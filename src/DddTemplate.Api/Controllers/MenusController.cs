using DddTemplate.Application.Menus;
using DddTemplate.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace DddTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly MenuService _menuService;
    private readonly ILogger<MenusController> _logger;

    public MenusController(MenuService menuService, ILogger<MenusController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("Fetching all menus");
        var menus = await _menuService.ListAsync(ct);
        return Ok(ApiResponse<List<MenuDto>>.SuccessResponse(menus.ToList()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Fetching menu with ID: {MenuId}", id);
        var menu = await _menuService.GetAsync(id, ct);

        if (menu == null)
        {
            _logger.LogWarning("Menu with ID {MenuId} not found", id);
            return NotFound(ApiResponse<MenuDto>.FailureResponse(
                new ErrorDetails("Menu.NotFound", $"Menu with ID {id} not found", "NotFound")));
        }

        return Ok(ApiResponse<MenuDto>.SuccessResponse(menu));
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create([FromBody] CreateMenuRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating menu with name: {Name}", request.Name);
        var menu = await _menuService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = menu.Id },
            ApiResponse<MenuDto>.SuccessResponse(menu));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateMenuRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Updating menu {MenuId}", id);
        var success = await _menuService.UpdateAsync(id, request, ct);

        if (!success)
        {
            _logger.LogWarning("Menu with ID {MenuId} not found", id);
            return NotFound(ApiResponse<bool>.FailureResponse(
                new ErrorDetails("Menu.NotFound", $"Menu with ID {id} not found", "NotFound")));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Deleting menu {MenuId}", id);
        var success = await _menuService.DeleteAsync(id, ct);

        if (!success)
        {
            _logger.LogWarning("Menu with ID {MenuId} not found", id);
            return NotFound(ApiResponse<bool>.FailureResponse(
                new ErrorDetails("Menu.NotFound", $"Menu with ID {id} not found", "NotFound")));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}
