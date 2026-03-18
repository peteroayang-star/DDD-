using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Application.TodoItems;
using DddTemplate.Application.Users;
using DddTemplate.Application.OperationLogs;
using DddTemplate.Application.Menus;
using DddTemplate.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DddTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 注册 HttpContextAccessor
        services.AddHttpContextAccessor();

        // 注册 MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 注册 FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // 注册应用服务
        services.AddScoped<TodoItemService>();
        services.AddScoped<UserService>();
        services.AddScoped<OperationLogService>();
        services.AddScoped<MenuService>();

        return services;
    }
}

