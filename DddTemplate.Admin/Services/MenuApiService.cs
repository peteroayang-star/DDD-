using DddTemplate.Admin.Models;
using System.Text;
using System.Text.Json;

namespace DddTemplate.Admin.Services;

public class MenuApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MenuApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public MenuApiService(IHttpClientFactory httpClientFactory, ILogger<MenuApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("DddTemplateApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<MenuDto>> GetAllMenusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/menus");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<MenuDto>>>(content, _jsonOptions);

            return apiResponse?.Data ?? new List<MenuDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单列表失败");
            throw;
        }
    }

    public async Task<MenuDto?> GetMenuByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/menus/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MenuDto>>(content, _jsonOptions);

            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单详情失败: {MenuId}", id);
            throw;
        }
    }

    public async Task CreateMenuAsync(MenuDto menu)
    {
        try
        {
            var json = JsonSerializer.Serialize(new
            {
                Name = menu.Name,
                Icon = menu.Icon,
                Path = menu.Path,
                ParentId = menu.ParentId,
                SortOrder = menu.SortOrder
            }, _jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/menus", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建菜单失败");
            throw;
        }
    }

    public async Task UpdateMenuAsync(Guid id, MenuDto menu)
    {
        try
        {
            var json = JsonSerializer.Serialize(new
            {
                Name = menu.Name,
                Icon = menu.Icon,
                Path = menu.Path,
                ParentId = menu.ParentId,
                SortOrder = menu.SortOrder
            }, _jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/menus/{id}", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新菜单失败: {MenuId}", id);
            throw;
        }
    }

    public async Task DeleteMenuAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/menus/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除菜单失败: {MenuId}", id);
            throw;
        }
    }
}
