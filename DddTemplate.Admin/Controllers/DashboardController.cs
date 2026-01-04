using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Services;

namespace DddTemplate.Admin.Controllers;

/// <summary>
/// 仪表盘控制器
/// 负责显示系统统计信息和快速操作入口
/// </summary>
public class DashboardController : Controller
{
    private readonly DashboardApiService _dashboardService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dashboardService">仪表盘 API 服务</param>
    public DashboardController(DashboardApiService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// 显示仪表盘首页
    /// </summary>
    /// <returns>仪表盘视图，包含系统统计数据</returns>
    public async Task<IActionResult> Index()
    {
        var statistics = await _dashboardService.GetStatisticsAsync();
        return View(statistics);
    }
}
