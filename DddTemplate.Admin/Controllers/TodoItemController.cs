using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Services;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Controllers;

/// <summary>
/// 待办事项控制器
/// 负责处理待办事项的 CRUD 操作
/// </summary>
public class TodoItemController : Controller
{
    private readonly TodoItemApiService _apiService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="apiService">待办事项 API 服务</param>
    public TodoItemController(TodoItemApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// 显示待办事项列表页面
    /// </summary>
    /// <returns>待办事项列表视图</returns>
    public async Task<IActionResult> Index()
    {
        var items = await _apiService.GetAllAsync();
        return View(items);
    }

    /// <summary>
    /// 显示新增待办事项页面
    /// </summary>
    /// <returns>新增待办事项视图</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// 处理新增待办事项的表单提交
    /// </summary>
    /// <param name="request">新增待办事项请求</param>
    /// <returns>成功则重定向到列表页，失败则返回表单页</returns>
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

    /// <summary>
    /// 显示编辑待办事项页面
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <returns>编辑待办事项视图，如果不存在则返回 404</returns>
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

    /// <summary>
    /// 处理编辑待办事项的表单提交
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <param name="request">更新待办事项请求</param>
    /// <returns>成功则重定向到列表页，失败则返回表单页</returns>
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

    /// <summary>
    /// 处理删除待办事项的请求
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <returns>重定向到列表页</returns>
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _apiService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// API: 创建待办事项（JSON 请求）
    /// </summary>
    /// <param name="request">新增待办事项请求</param>
    /// <returns>创建的待办事项</returns>
    [HttpPost]
    [Route("/api/todoitems")]
    public async Task<IActionResult> ApiCreate([FromBody] CreateTodoItemRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _apiService.CreateAsync(request);
        if (result == null)
            return BadRequest(new { message = "创建失败" });

        return Ok(result);
    }

    /// <summary>
    /// API: 更新待办事项（JSON 请求）
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <param name="request">更新待办事项请求</param>
    /// <returns>更新结果</returns>
    [HttpPut]
    [Route("/api/todoitems/{id}")]
    public async Task<IActionResult> ApiUpdate(Guid id, [FromBody] UpdateTodoItemRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _apiService.UpdateAsync(id, request);
        if (!success)
            return BadRequest(new { message = "更新失败" });

        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// API: 标记待办事项为完成（JSON 请求）
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [Route("/api/todoitems/{id}/complete")]
    public async Task<IActionResult> ApiComplete(Guid id)
    {
        var success = await _apiService.CompleteAsync(id);
        if (!success)
            return BadRequest(new { message = "操作失败" });

        return Ok(new { message = "标记完成成功" });
    }
}
