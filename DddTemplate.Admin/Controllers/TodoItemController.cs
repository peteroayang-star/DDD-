using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Services;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Controllers;

public class TodoItemController : Controller
{
    private readonly TodoItemApiService _apiService;

    public TodoItemController(TodoItemApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _apiService.GetAllAsync();
        return View(items);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoItemRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _apiService.CreateAsync(request);
        if (result == null)
        {
            ModelState.AddModelError("", "创建失败");
            return View(request);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _apiService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        var request = new UpdateTodoItemRequest
        {
            Title = item.Title,
            Description = item.Description
        };

        ViewBag.Id = id;
        return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateTodoItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Id = id;
            return View(request);
        }

        var success = await _apiService.UpdateAsync(id, request);
        if (!success)
        {
            ModelState.AddModelError("", "更新失败");
            ViewBag.Id = id;
            return View(request);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _apiService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
