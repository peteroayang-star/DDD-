using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Services;

namespace DddTemplate.Admin.Controllers;

public class TestController : Controller
{
    private readonly VisitStatisticsService _visitService;

    public TestController(VisitStatisticsService visitService)
    {
        _visitService = visitService;
    }

    public async Task<IActionResult> Index()
    {
        var todayVisits = await _visitService.GetTodayVisits();

        return Content($"今日访问: {todayVisits}");
    }
}
