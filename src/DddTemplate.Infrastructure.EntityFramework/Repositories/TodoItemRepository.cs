using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Infrastructure.EntityFramework.Repositories;

public class TodoItemRepository : EfRepository<TodoItem, Guid>, ITodoItemRepository
{
    public TodoItemRepository(ApplicationDbContext context) : base(context)
    {
    }
}
