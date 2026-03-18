using Microsoft.AspNetCore.Mvc;
using DddTemplate.Admin.Models;

namespace DddTemplate.Admin.Controllers;

public class RoleController : Controller
{
    private static readonly List<RoleDto> _roles = new()
    {
        new RoleDto { Id = Guid.NewGuid(), Name = "管理员", Description = "系统管理员", CreatedAt = DateTime.Now },
        new RoleDto { Id = Guid.NewGuid(), Name = "用户", Description = "普通用户", CreatedAt = DateTime.Now }
    };

    public IActionResult Index()
    {
        return View(_roles);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string name, string description)
    {
        _roles.Add(new RoleDto
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.Now
        });
        return RedirectToAction(nameof(Index));
    }
}
