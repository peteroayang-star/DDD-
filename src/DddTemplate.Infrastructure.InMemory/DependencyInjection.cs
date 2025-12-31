using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;
using DddTemplate.Domain.Users;
using DddTemplate.Infrastructure.InMemory.TodoItems;
using DddTemplate.Infrastructure.InMemory.Users;
using Microsoft.Extensions.DependencyInjection;

namespace DddTemplate.Infrastructure.InMemory;

public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services)
    {
        // 注册仓储实现
        services.AddSingleton<ITodoItemRepository, InMemoryTodoItemRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        // InMemory 数据通常全局共享，用单例即可
        return services;
    }
}

