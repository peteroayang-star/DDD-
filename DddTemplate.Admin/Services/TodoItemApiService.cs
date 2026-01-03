using System.Text;
using System.Text.Json;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Services;

public class TodoItemApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public TodoItemApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<TodoItemDto>> GetAllAsync()
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.GetAsync("/api/todoitems");

        if (!response.IsSuccessStatusCode)
            return new List<TodoItemDto>();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TodoItemDto>>>(content, _jsonOptions);

        return apiResponse?.Data ?? new List<TodoItemDto>();
    }

    public async Task<TodoItemDto?> GetByIdAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.GetAsync($"/api/todoitems/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TodoItemDto>>(content, _jsonOptions);

        return apiResponse?.Data;
    }

    public async Task<TodoItemDto?> CreateAsync(CreateTodoItemRequest request)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/todoitems", httpContent);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TodoItemDto>>(content, _jsonOptions);

        return apiResponse?.Data;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTodoItemRequest request)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"/api/todoitems/{id}", httpContent);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.DeleteAsync($"/api/todoitems/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CompleteAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("DddTemplateApi");
        var response = await client.PostAsync($"/api/todoitems/{id}/complete", null);
        return response.IsSuccessStatusCode;
    }
}
