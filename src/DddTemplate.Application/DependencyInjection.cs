using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Application.TodoItems;
using DddTemplate.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace DddTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 注册应用服务
        services.AddScoped<TodoItemService>();
        services.AddScoped<UserService>();

        return services;
    }
}

