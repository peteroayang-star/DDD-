namespace DddTemplate.Admin.Models;

public class DashboardStatistics
{
    public int TotalUsers { get; set; }
    public int TotalTodoItems { get; set; }
    public int CompletedTodoItems { get; set; }
    public int PendingTodoItems { get; set; }
    public int TodayVisits { get; set; }
    public int SystemMessages { get; set; }
}
