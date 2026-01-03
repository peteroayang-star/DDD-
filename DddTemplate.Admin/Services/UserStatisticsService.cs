namespace DddTemplate.Admin.Services;

public class UserStatisticsService
{
    private static readonly HashSet<string> _registeredUsers = new HashSet<string>();
    private static readonly object _lock = new object();

    public void RegisterUser(string username)
    {
        lock (_lock)
        {
            _registeredUsers.Add(username);
        }
    }

    public int GetTotalUsers()
    {
        lock (_lock)
        {
            return _registeredUsers.Count;
        }
    }
}
