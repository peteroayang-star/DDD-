using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DddTemplate.Domain.TodoItems;
using DddTemplate.Domain.Users;
using DddTemplate.Infrastructure.EntityFramework.Repositories;

namespace DddTemplate.Infrastructure.EntityFramework;

public static class DependencyInjection
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
