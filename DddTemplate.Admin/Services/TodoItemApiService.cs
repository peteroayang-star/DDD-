using System.Text;
using System.Text.Json;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Services;

/// <summary>
/// 待办事项 API 服务
/// 负责与后端 API 进行通信，处理待办事项的 CRUD 操作
/// </summary>
public class TodoItemApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="httpClientFactory">HTTP 客户端工厂</param>
    public TodoItemApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// 获取所有待办事项
    /// </summary>
    /// <returns>待办事项列表</returns>
    public async Task<List<TodoItemDto>> GetAllAsync()
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.GetAsync("/api/todos");

        if (!response.IsSuccessStatusCode)
            return new List<TodoItemDto>();

        var content = await response.Content.ReadAsStringAsync();

        // API 直接返回列表，不是 ApiResponse 包装格式
        var items = JsonSerializer.Deserialize<List<TodoItemDto>>(content, _jsonOptions);

        return items ?? new List<TodoItemDto>();
    }

    /// <summary>
    /// 根据 ID 获取待办事项
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <returns>待办事项详情，如果不存在则返回 null</returns>
    public async Task<TodoItemDto?> GetByIdAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.GetAsync($"/api/todos/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();

        // API 直接返回对象，不是 ApiResponse 包装格式
        var item = JsonSerializer.Deserialize<TodoItemDto>(content, _jsonOptions);

        return item;
    }

    /// <summary>
    /// 创建新的待办事项
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建成功返回待办事项详情，失败返回 null</returns>
    public async Task<TodoItemDto?> CreateAsync(CreateTodoItemRequest request)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/todos", httpContent);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();

        // API 直接返回创建的对象
        var item = JsonSerializer.Deserialize<TodoItemDto>(content, _jsonOptions);

        return item;
    }

    /// <summary>
    /// 更新待办事项
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新成功返回 true，失败返回 false</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateTodoItemRequest request)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"/api/todos/{id}", httpContent);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.DeleteAsync($"/api/todos/{id}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// 标记待办事项为已完成
    /// </summary>
    /// <param name="id">待办事项 ID</param>
    /// <returns>标记成功返回 true，失败返回 false</returns>
    public async Task<bool> CompleteAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.PutAsync($"/api/todos/{id}/complete", null);
        return response.IsSuccessStatusCode;
    }
}
