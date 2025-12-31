# 第二阶段实施计划

## 📊 当前状态

### ✅ 已完成的工作

#### 第一阶段（100% 完成）
1. ✅ 领域层核心抽象
   - AggregateRoot、Entity、ValueObject
   - IDomainEvent、DomainEvent
   - IAggregateRoot、IDomainService
   - 3个领域事件示例（Created、Completed、Renamed）

2. ✅ Result 模式和错误处理
   - Result 和 Result<T> 类
   - Error 类型系统（6种错误类型）
   - 自定义领域异常（4种）
   - 全局异常处理中间件
   - 统一 API 响应格式

3. ✅ 文档
   - CLAUDE.md（完整使用指南）

#### 第二阶段（部分完成）
1. ✅ CQRS 核心抽象
   - ICommand 和 ICommand<TResponse>
   - ICommandHandler<TCommand, TResponse>
   - IQuery<TResponse>
   - IQueryHandler<TQuery, TResponse>

2. ✅ MediatR NuGet 包
   - MediatR 14.0.0 已添加到 Application 项目

### 🔄 待完成的工作

#### 第二阶段剩余任务

1. **FluentValidation 集成**
   - 添加 FluentValidation NuGet 包
   - 创建验证器基类
   - 创建具体验证器示例

2. **MediatR 集成和配置**
   - 更新 ICommand/IQuery 接口以继承 MediatR.IRequest
   - 更新 Handler 接口以继承 MediatR.IRequestHandler
   - 配置 MediatR 依赖注入
   - 创建管道行为（Validation、Logging、Transaction）

3. **CQRS 具体实现**
   - 创建 TodoItem Commands（Create, Complete, Rename）
   - 创建 TodoItem Queries（GetById, GetAll）
   - 创建对应的 Handlers
   - 创建对应的 Validators

4. **更新 API 层**
   - 注入 IMediator
   - 更新端点使用 MediatR.Send()
   - 移除直接依赖 Application Services

5. **领域事件发布**
   - 创建 IDomainEventPublisher 接口
   - 实现 MediatR 领域事件发布器
   - 在仓储保存后发布事件

---

## 🚀 详细实施步骤

### 步骤 1：添加 FluentValidation

```bash
cd "D:\创作\DDD 基础架构\src\DddTemplate.Application"
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

### 步骤 2：更新 CQRS 接口以集成 MediatR

**更新 ICommand.cs**:
```csharp
using MediatR;

namespace DddTemplate.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
```

**更新 IQuery.cs**:
```csharp
using MediatR;

namespace DddTemplate.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
```

**更新 ICommandHandler.cs**:
```csharp
using MediatR;

namespace DddTemplate.Application.Abstractions.Messaging;

public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}
```

**更新 IQueryHandler.cs**:
```csharp
using MediatR;

namespace DddTemplate.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
```

### 步骤 3：创建 Commands

**CreateTodoItemCommand.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;
using DddTemplate.Application.TodoItems;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed record CreateTodoItemCommand(string Title)
    : ICommand<TodoItemDto>;
```

**CompleteTodoItemCommand.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed record CompleteTodoItemCommand(Guid Id) : ICommand;
```

**RenameTodoItemCommand.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed record RenameTodoItemCommand(Guid Id, string NewTitle) : ICommand;
```

### 步骤 4：创建 Queries

**GetTodoItemByIdQuery.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;
using DddTemplate.Application.TodoItems;

namespace DddTemplate.Application.TodoItems.Queries;

public sealed record GetTodoItemByIdQuery(Guid Id)
    : IQuery<TodoItemDto>;
```

**GetAllTodoItemsQuery.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;
using DddTemplate.Application.TodoItems;

namespace DddTemplate.Application.TodoItems.Queries;

public sealed record GetAllTodoItemsQuery
    : IQuery<IReadOnlyList<TodoItemDto>>;
```

### 步骤 5：创建 Command Handlers

**CreateTodoItemCommandHandler.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;
using DddTemplate.Application.TodoItems;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems.Commands;

internal sealed class CreateTodoItemCommandHandler
    : ICommandHandler<CreateTodoItemCommand, TodoItemDto>
{
    private readonly ITodoItemRepository _repository;

    public CreateTodoItemCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TodoItemDto>> Handle(
        CreateTodoItemCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var todoItem = TodoItem.Create(command.Title);
            await _repository.AddAsync(todoItem, cancellationToken);

            var dto = new TodoItemDto(
                todoItem.Id,
                todoItem.Title,
                todoItem.IsCompleted,
                todoItem.CreatedAt
            );

            return Result.Success(dto);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TodoItemDto>(
                Error.Validation("TodoItem.InvalidTitle", ex.Message)
            );
        }
    }
}
```

**CompleteTodoItemCommandHandler.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems.Commands;

internal sealed class CompleteTodoItemCommandHandler
    : ICommandHandler<CompleteTodoItemCommand>
{
    private readonly ITodoItemRepository _repository;

    public CompleteTodoItemCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        CompleteTodoItemCommand command,
        CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure(
                Error.NotFound("TodoItem.NotFound", $"TodoItem with ID {command.Id} not found")
            );
        }

        todoItem.MarkCompleted();

        return Result.Success();
    }
}
```

### 步骤 6：创建 Query Handlers

**GetTodoItemByIdQueryHandler.cs**:
```csharp
using DddTemplate.Application.Abstractions.Messaging;
using DddTemplate.Application.TodoItems;
using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Application.TodoItems.Queries;

internal sealed class GetTodoItemByIdQueryHandler
    : IQueryHandler<GetTodoItemByIdQuery, TodoItemDto>
{
    private readonly ITodoItemRepository _repository;

    public GetTodoItemByIdQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TodoItemDto>> Handle(
        GetTodoItemByIdQuery query,
        CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<TodoItemDto>(
                Error.NotFound("TodoItem.NotFound", $"TodoItem with ID {query.Id} not found")
            );
        }

        var dto = new TodoItemDto(
            todoItem.Id,
            todoItem.Title,
            todoItem.IsCompleted,
            todoItem.CreatedAt
        );

        return Result.Success(dto);
    }
}
```

### 步骤 7：创建 Validators

**CreateTodoItemCommandValidator.cs**:
```csharp
using FluentValidation;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed class CreateTodoItemCommandValidator
    : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");
    }
}
```

### 步骤 8：创建管道行为

**ValidationBehavior.cs**:
```csharp
using FluentValidation;
using MediatR;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray()
                );

            var error = Error.Validation(
                "Validation.Failed",
                "One or more validation errors occurred"
            );

            // 需要创建一个带验证错误的 Result
            return CreateValidationResult<TResponse>(error, errors);
        }

        return await next();
    }

    private static TResponse CreateValidationResult<T>(
        Error error,
        Dictionary<string, string[]> errors)
        where T : Result
    {
        if (typeof(T) == typeof(Result))
        {
            return (Result.Failure(error) as T)!;
        }

        var validationResult = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(T).GenericTypeArguments[0])
            .GetMethod(nameof(Result.Failure))!
            .Invoke(null, new object[] { error })!;

        return (T)validationResult;
    }
}
```

**LoggingBehavior.cs**:
```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Processing request {RequestName}",
            requestName
        );

        var result = await next();

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Request {RequestName} processed successfully",
                requestName
            );
        }
        else
        {
            _logger.LogWarning(
                "Request {RequestName} failed with error: {Error}",
                requestName,
                result.Error
            );
        }

        return result;
    }
}
```

### 步骤 9：更新 DependencyInjection.cs

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using DddTemplate.Application.Behaviors;

namespace DddTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 注册 MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // 添加管道行为
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // 注册 FluentValidation 验证器
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true
        );

        return services;
    }
}
```

### 步骤 10：更新 API 端点

**Program.cs**:
```csharp
using MediatR;
using DddTemplate.Application.TodoItems.Commands;
using DddTemplate.Application.TodoItems.Queries;

// 注入 IMediator
app.MapPost("/api/todos", async (
    CreateTodoItemCommand command,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(command, ct);

    return result.IsSuccess
        ? Results.Created($"/api/todos/{result.Value.Id}", result.Value)
        : Results.BadRequest(result.Error);
})
.WithName("CreateTodo")
.WithTags("Todos");

app.MapGet("/api/todos/{id:guid}", async (
    Guid id,
    IMediator mediator,
    CancellationToken ct) =>
{
    var query = new GetTodoItemByIdQuery(id);
    var result = await mediator.Send(query, ct);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.NotFound(result.Error);
})
.WithName("GetTodoById")
.WithTags("Todos");

app.MapGet("/api/todos", async (
    IMediator mediator,
    CancellationToken ct) =>
{
    var query = new GetAllTodoItemsQuery();
    var result = await mediator.Send(query, ct);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(result.Error);
})
.WithName("GetTodos")
.WithTags("Todos");

app.MapPut("/api/todos/{id:guid}/complete", async (
    Guid id,
    IMediator mediator,
    CancellationToken ct) =>
{
    var command = new CompleteTodoItemCommand(id);
    var result = await mediator.Send(command, ct);

    return result.IsSuccess
        ? Results.NoContent()
        : Results.NotFound(result.Error);
})
.WithName("CompleteTodo")
.WithTags("Todos");
```

---

## 🎯 实施优先级

### 高优先级（核心功能）
1. ✅ CQRS 接口（已完成）
2. ⏳ MediatR 集成
3. ⏳ 创建 Commands 和 Queries
4. ⏳ 创建 Handlers
5. ⏳ 更新 API 端点

### 中优先级（增强功能）
6. ⏳ FluentValidation 集成
7. ⏳ 验证管道行为
8. ⏳ 日志管道行为

### 低优先级（可选功能）
9. ⏳ 领域事件发布
10. ⏳ 事务管道行为
11. ⏳ 性能监控管道行为

---

## 📝 注意事项

1. **MediatR 版本**: 使用 MediatR 14.0.0，接口有所变化
2. **Result 模式**: 所有 Handler 返回 Result 或 Result<T>
3. **验证**: FluentValidation 在管道中自动执行
4. **日志**: 使用 ILogger 记录请求处理过程
5. **异常**: 领域异常仍由全局中间件处理

---

## 🚀 快速开始

完成第二阶段后，使用方式：

```csharp
// 在 API 端点中
public async Task<IResult> CreateTodo(
    CreateTodoItemCommand command,
    IMediator mediator,
    CancellationToken ct)
{
    var result = await mediator.Send(command, ct);
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(result.Error);
}
```

---

## 📚 参考资源

- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
