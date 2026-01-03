namespace DddTemplate.Admin.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // 排除登录页面和静态资源
        if (path.Contains("/account/login") || path.Contains("/css/") || 
            path.Contains("/js/") || path.Contains("/lib/"))
        {
            await _next(context);
            return;
        }
        
        // 检查是否有登录Cookie
        if (!context.Request.Cookies.ContainsKey("UserToken"))
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
        
        // 实现滑动过期：每次请求都刷新Cookie过期时间
        var username = context.Request.Cookies["UserName"];
        if (!string.IsNullOrEmpty(username))
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(2) // 重置为2小时后过期
            };
            
            // 刷新Cookie过期时间
            context.Response.Cookies.Append("UserToken", username, cookieOptions);
            context.Response.Cookies.Append("UserName", username, cookieOptions);
        }
        
        await _next(context);
    }
}
