using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Services;

namespace DddTemplate.Admin.Controllers;

public class DashboardController : Controller
{
    private readonly DashboardApiService _dashboardService;

    public DashboardController(DashboardApiService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var statistics = await _dashboardService.GetStatisticsAsync();
        return View(statistics);
    }
}
