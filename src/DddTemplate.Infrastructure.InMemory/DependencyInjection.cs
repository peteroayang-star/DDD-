using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;
using DddTemplate.Domain.Users;
using DddTemplate.Domain.OperationLogs;
using DddTemplate.Domain.Menus;
using DddTemplate.Infrastructure.InMemory.TodoItems;
using DddTemplate.Infrastructure.InMemory.Users;
using DddTemplate.Infrastructure.InMemory.OperationLogs;
using DddTemplate.Infrastructure.InMemory.Menus;
using Microsoft.Extensions.DependencyInjection;

namespace DddTemplate.Infrastructure.InMemory;

public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services)
    {
        // 注册仓储实现
        services.AddSingleton<ITodoItemRepository, InMemoryTodoItemRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IOperationLogRepository, OperationLogRepository>();
        services.AddSingleton<IMenuRepository, MenuRepository>();

        // InMemory 数据通常全局共享，用单例即可
        return services;
    }
}

