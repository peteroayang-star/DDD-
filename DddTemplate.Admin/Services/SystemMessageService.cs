namespace DddTemplate.Admin.Services;

/// <summary>
/// 系统消息服务
/// 用于管理系统消息和通知
/// </summary>
public class SystemMessageService
{
    private static int _unreadMessages = 3; // 初始化一些未读消息
    private static readonly object _lock = new object();

    /// <summary>
    /// 添加一条新消息
    /// </summary>
    public void AddMessage()
    {
        lock (_lock)
        {
            _unreadMessages++;
        }
    }

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    /// <returns>未读消息数</returns>
    public int GetUnreadCount()
    {
        lock (_lock)
        {
            return _unreadMessages;
        }
    }

    /// <summary>
    /// 清空所有消息
    /// </summary>
    public void ClearMessages()
    {
        lock (_lock)
        {
            _unreadMessages = 0;
        }
    }
}
