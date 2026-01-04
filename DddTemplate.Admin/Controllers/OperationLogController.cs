using DddTemplate.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace DddTemplate.Admin.Controllers;

/// <summary>
/// 操作日志控制器
/// </summary>
public class OperationLogController : Controller
{
    private readonly OperationLogApiService _operationLogService;
    private readonly ILogger<OperationLogController> _logger;

    public OperationLogController(
        OperationLogApiService operationLogService,
        ILogger<OperationLogController> logger)
    {
        _operationLogService = operationLogService;
        _logger = logger;
    }

    /// <summary>
    /// 操作日志列表页面
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var logs = await _operationLogService.GetAllAsync();
            return View(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading operation logs");
            return View(new List<Models.OperationLogDto>());
        }
    }

    /// <summary>
    /// 操作日志详情页面
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var log = await _operationLogService.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            return View(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading operation log {LogId}", id);
            return NotFound();
        }
    }
}
