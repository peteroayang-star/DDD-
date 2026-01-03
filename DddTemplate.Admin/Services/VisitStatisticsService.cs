namespace DddTemplate.Admin.Services;

public class VisitStatisticsService
{
    private static int _totalVisits = 0;
    private static int _todayVisits = 0;
    private static DateTime _lastResetDate = DateTime.Today;
    private static readonly object _lock = new object();

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

    public int GetTotalVisits()
    {
        lock (_lock)
        {
            return _totalVisits;
        }
    }

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
