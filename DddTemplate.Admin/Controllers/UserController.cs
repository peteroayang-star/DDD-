using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Controllers;

public class UserController : Controller
{
    private readonly HttpClient _httpClient;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5000");
    }

    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<UserDto>>>("api/users");
        var users = response?.Data ?? new();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _httpClient.DeleteAsync($"api/users/{id}");
        return RedirectToAction(nameof(Index));
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
}
