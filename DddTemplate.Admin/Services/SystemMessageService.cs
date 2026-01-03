namespace DddTemplate.Admin.Services;

public class SystemMessageService
{
    private static int _unreadMessages = 0;
    private static readonly object _lock = new object();

    public void AddMessage()
    {
        lock (_lock)
        {
            _unreadMessages++;
        }
    }

    public int GetUnreadCount()
    {
        lock (_lock)
        {
            return _unreadMessages;
        }
    }

    public void ClearMessages()
    {
        lock (_lock)
        {
            _unreadMessages = 0;
        }
    }
}
