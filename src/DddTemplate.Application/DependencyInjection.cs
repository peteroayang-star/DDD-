using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Application.TodoItems;
using Microsoft.Extensions.DependencyInjection;

namespace DddTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TodoItemService>();
        // 后面有更多 Service 再加
        return services;
    }
}

