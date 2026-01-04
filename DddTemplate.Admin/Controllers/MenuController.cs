using DddTemplate.Admin.Models;
using DddTemplate.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace DddTemplate.Admin.Controllers;

public class MenuController : Controller
{
    private readonly MenuApiService _menuApiService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(MenuApiService menuApiService, ILogger<MenuController> logger)
    {
        _menuApiService = menuApiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var menus = await _menuApiService.GetAllMenusAsync();
            return View(menus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单列表失败");
            TempData["Error"] = "获取菜单列表失败";
            return View(new List<MenuDto>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MenuDto model)
    {
        try
        {
            await _menuApiService.CreateMenuAsync(model);
            TempData["Success"] = "菜单创建成功";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建菜单失败");
            TempData["Error"] = "创建菜单失败";
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var menu = await _menuApiService.GetMenuByIdAsync(id);
            if (menu == null)
            {
                TempData["Error"] = "菜单不存在";
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单详情失败");
            TempData["Error"] = "获取菜单详情失败";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, MenuDto model)
    {
        try
        {
            await _menuApiService.UpdateMenuAsync(id, model);
            TempData["Success"] = "菜单更新成功";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新菜单失败");
            TempData["Error"] = "更新菜单失败";
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _menuApiService.DeleteMenuAsync(id);
            TempData["Success"] = "菜单删除成功";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除菜单失败");
            TempData["Error"] = "删除菜单失败";
            return RedirectToAction(nameof(Index));
        }
    }
}
