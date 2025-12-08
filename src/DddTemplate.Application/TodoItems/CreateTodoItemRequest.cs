using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DddTemplate.Application.TodoItems;

public sealed record CreateTodoItemRequest(string Title);

