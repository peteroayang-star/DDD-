using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Controllers;

public class UserController : Controller
{
    private readonly HttpClient _httpClient;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5001");
    }

    public async Task<IActionResult> Index()
    {
        var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users") ?? new();
        return View(users);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("", "创建用户失败");
        return View(request);
    }
}
