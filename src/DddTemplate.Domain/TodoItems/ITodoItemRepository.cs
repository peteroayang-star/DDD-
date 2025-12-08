using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Domain.TodoItems;

public interface ITodoItemRepository : IRepository<TodoItem, Guid>
{
    // 如果后面有针对 Todo 的特殊查询，再慢慢加
}
