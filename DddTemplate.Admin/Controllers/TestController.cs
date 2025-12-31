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

    public IActionResult Index()
    {
        var todayVisits = _visitService.GetTodayVisits();
        var totalVisits = _visitService.GetTotalVisits();
        
        return Content($"今日访问: {todayVisits}, 总访问: {totalVisits}");
    }
}
