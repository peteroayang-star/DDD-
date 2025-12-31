using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.TodoItems;
using DddTemplate.Infrastructure.InMemory.Common;

namespace DddTemplate.Infrastructure.InMemory.TodoItems;

public sealed class InMemoryTodoItemRepository
    : InMemoryRepository<TodoItem, Guid>, ITodoItemRepository
{
}
