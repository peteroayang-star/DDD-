namespace DddTemplate.Admin.Services;

/// <summary>
/// 访问统计服务
/// 用于记录和统计网站访问量
/// </summary>
public class VisitStatisticsService
{
    private static int _totalVisits = 128; // 初始化一些模拟访问量
    private static int _todayVisits = 23;  // 今日访问量
    private static DateTime _lastResetDate = DateTime.Today;
    private static readonly object _lock = new object();

    /// <summary>
    /// 记录一次访问
    /// </summary>
    public void RecordVisit()
    {
        lock (_lock)
        {
            // 检查是否需要重置今日访问数
            if (DateTime.Today > _lastResetDate)
            {
                _todayVisits = 0;
                _lastResetDate = DateTime.Today;
            }

            _totalVisits++;
            _todayVisits++;
        }
    }

    /// <summary>
    /// 获取总访问量
    /// </summary>
    /// <returns>总访问次数</returns>
    public int GetTotalVisits()
    {
        lock (_lock)
        {
            return _totalVisits;
        }
    }

    /// <summary>
    /// 获取今日访问量
    /// </summary>
    /// <returns>今日访问次数</returns>
    public int GetTodayVisits()
    {
        lock (_lock)
        {
            // 检查是否需要重置
            if (DateTime.Today > _lastResetDate)
            {
                _todayVisits = 0;
                _lastResetDate = DateTime.Today;
            }
            return _todayVisits;
        }
    }
}
