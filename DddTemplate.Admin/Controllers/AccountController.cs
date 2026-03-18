using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Services;

namespace DddTemplate.Admin.Controllers;

public class AccountController : Controller
{
    private readonly VisitStatisticsService _visitService;
    private readonly UserStatisticsService _userService;

    public AccountController(VisitStatisticsService visitService, UserStatisticsService userService)
    {
        _visitService = visitService;
        _userService = userService;
    }

    public IActionResult Login()
    {
        // 如果已登录，跳转到仪表盘
        if (HttpContext.Request.Cookies.ContainsKey("UserToken"))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        // 简单验证（实际项目应该调用API验证）
        if (username == "admin" && password == "admin123")
        {
            // 设置Cookie，2小时过期，滑动过期
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // 开发环境设为false，生产环境应为true
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            };

            HttpContext.Response.Cookies.Append("UserToken", username, cookieOptions);
            HttpContext.Response.Cookies.Append("UserName", username, cookieOptions);

            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Error = "用户名或密码错误";
        return View();
    }

    public IActionResult Logout()
    {
        // 删除Cookie
        HttpContext.Response.Cookies.Delete("UserToken");
        HttpContext.Response.Cookies.Delete("UserName");
        
        return RedirectToAction("Login");
    }
}
