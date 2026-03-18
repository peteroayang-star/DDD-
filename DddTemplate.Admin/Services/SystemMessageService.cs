namespace DddTemplate.Admin.Services;

public class SystemMessageService
{
    private readonly TodoItemApiService _todoService;

    public SystemMessageService(TodoItemApiService todoService)
    {
        _todoService = todoService;
    }

    public async Task<int> GetUnreadCount()
    {
        try
        {
            var todos = await _todoService.GetAllAsync();
            return todos.Count(t => !t.IsCompleted);
        }
        catch
        {
            return 0;
        }
    }
}
