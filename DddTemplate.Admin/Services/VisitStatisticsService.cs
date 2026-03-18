namespace DddTemplate.Admin.Services;

public class VisitStatisticsService
{
    private readonly HttpClient _httpClient;

    public VisitStatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> GetTodayVisits()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<object>>("http://localhost:5001/api/operation-logs");
            return response?.Count ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}
