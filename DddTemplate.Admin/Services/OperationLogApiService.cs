using DddTemplate.Admin.Models;
using System.Text.Json;

namespace DddTemplate.Admin.Services;

/// <summary>
/// 操作日志 API 服务
/// </summary>
public class OperationLogApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OperationLogApiService> _logger;

    public OperationLogApiService(IHttpClientFactory httpClientFactory, ILogger<OperationLogApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有操作日志
    /// </summary>
    public async Task<List<OperationLogDto>> GetAllAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("DddTemplateApi");
            var response = await client.GetAsync("/api/operation-logs");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var logs = JsonSerializer.Deserialize<List<OperationLogDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return logs ?? new List<OperationLogDto>();
            }

            _logger.LogWarning("Failed to fetch operation logs. Status: {StatusCode}", response.StatusCode);
            return new List<OperationLogDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching operation logs");
            return new List<OperationLogDto>();
        }
    }

    /// <summary>
    /// 根据 ID 获取操作日志
    /// </summary>
    public async Task<OperationLogDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("DddTemplateApi");
            var response = await client.GetAsync($"/api/operation-logs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OperationLogDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching operation log {LogId}", id);
            return null;
        }
    }
}
