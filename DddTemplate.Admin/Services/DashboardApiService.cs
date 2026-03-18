using System.Text.Json;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Services;

public class DashboardApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly VisitStatisticsService _visitService;
    private readonly UserStatisticsService _userService;
    private readonly SystemMessageService _messageService;
    private readonly JsonSerializerOptions _jsonOptions;

    public DashboardApiService(
        IHttpClientFactory httpClientFactory,
        VisitStatisticsService visitService,
        UserStatisticsService userService,
        SystemMessageService messageService)
    {
        _httpClientFactory = httpClientFactory;
        _visitService = visitService;
        _userService = userService;
        _messageService = messageService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<DashboardStatistics> GetStatisticsAsync()
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        
        // 获取待办事项统计
        var todoResponse = await client.GetAsync("/api/todoitems");
        var todoItems = new List<TodoItemDto>();
        
        if (todoResponse.IsSuccessStatusCode)
        {
            var content = await todoResponse.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TodoItemDto>>>(content, _jsonOptions);
            todoItems = apiResponse?.Data ?? new List<TodoItemDto>();
        }

        return new DashboardStatistics
        {
            TotalUsers = await _userService.GetTotalUsers(),
            TotalTodoItems = todoItems.Count,
            CompletedTodoItems = todoItems.Count(x => x.IsCompleted),
            PendingTodoItems = todoItems.Count(x => !x.IsCompleted),
            TodayVisits = await _visitService.GetTodayVisits(),
            SystemMessages = await _messageService.GetUnreadCount()
        };
    }
}
