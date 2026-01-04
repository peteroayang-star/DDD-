using Microsoft.AspNetCore.Http;

namespace DddTemplate.Admin.Helpers;

/// <summary>
/// IP 地址辅助类
/// </summary>
public static class IpAddressHelper
{
    /// <summary>
    /// 获取客户端真实 IP 地址
    /// </summary>
    public static string? GetClientIpAddress(HttpContext httpContext)
    {
        if (httpContext == null)
            return null;

        // 1. 尝试从 X-Forwarded-For 头获取（代理/负载均衡场景）
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For 可能包含多个 IP，取第一个
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // 2. 尝试从 X-Real-IP 头获取（Nginx 代理场景）
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp.Trim();
        }

        // 3. 直接从连接信息获取
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress != null)
        {
            // 如果是 IPv6 的 localhost，转换为 IPv4
            if (remoteIpAddress.ToString() == "::1")
            {
                return "127.0.0.1";
            }

            return remoteIpAddress.ToString();
        }

        return null;
    }
}
