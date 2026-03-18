namespace DddTemplate.Admin.Services;

public class UserStatisticsService
{
    private readonly HttpClient _httpClient;

    public UserStatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> GetTotalUsers()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<object>>("http://localhost:5001/api/users");
            return response?.Count ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}
