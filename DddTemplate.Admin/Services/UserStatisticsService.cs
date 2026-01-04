namespace DddTemplate.Admin.Services;

/// <summary>
/// 用户统计服务
/// 用于统计和管理用户数据（模拟数据）
/// </summary>
public class UserStatisticsService
{
    private static readonly HashSet<string> _registeredUsers = new HashSet<string>
    {
        // 初始化一些模拟用户
        "admin",
        "user1",
        "user2",
        "testuser",
        "demo"
    };
    private static readonly object _lock = new object();

    /// <summary>
    /// 注册新用户
    /// </summary>
    /// <param name="username">用户名</param>
    public void RegisterUser(string username)
    {
        lock (_lock)
        {
            _registeredUsers.Add(username);
        }
    }

    /// <summary>
    /// 获取总用户数
    /// </summary>
    /// <returns>用户总数</returns>
    public int GetTotalUsers()
    {
        lock (_lock)
        {
            return _registeredUsers.Count;
        }
    }
}
