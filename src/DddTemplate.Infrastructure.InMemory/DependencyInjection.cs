using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;
using DddTemplate.Infrastructure.InMemory.TodoItems;
using Microsoft.Extensions.DependencyInjection;

namespace DddTemplate.Infrastructure.InMemory;

public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITodoItemRepository, InMemoryTodoItemRepository>();
        // InMemory 数据通常全局共享，用单例即可
        return services;
    }
}

