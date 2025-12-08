# DDD 基础架构 - 架构设计文档

## 📐 整体架构图

```
┌─────────────────────────────────────────────────────────────────────┐
│                          客户端 (Client)                              │
│                    Browser / Mobile / Desktop                        │
└────────────────────────────┬────────────────────────────────────────┘
                             │ HTTP/HTTPS
                             ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    表现层 (Presentation Layer)                        │
│                      DddTemplate.Api                                 │
├─────────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              │
│  │ Minimal API  │  │   Swagger    │  │  Middleware  │              │
│  │  Endpoints   │  │     UI       │  │   Pipeline   │              │
│  └──────────────┘  └──────────────┘  └──────────────┘              │
│         │                                     │                      │
│         │          ┌──────────────────────────┘                      │
│         │          │ GlobalExceptionHandler                          │
│         │          │ ApiResponse<T>                                  │
└─────────┼──────────┴─────────────────────────────────────────────────┘
          │ 依赖注入
          ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    应用层 (Application Layer)                         │
│                   DddTemplate.Application                            │
├─────────────────────────────────────────────────────────────────────┤
│  ┌──────────────────┐  ┌──────────────────┐  ┌─────────────────┐   │
│  │ Application      │  │  CQRS Abstracts  │  │      DTOs       │   │
│  │   Services       │  │ Commands/Queries │  │  TodoItemDto    │   │
│  │ TodoItemService  │  │    Handlers      │  │                 │   │
│  └──────────────────┘  └──────────────────┘  └─────────────────┘   │
│         │                      │                                     │
│         └──────────┬───────────┘                                     │
│                    │ 调用                                             │
└────────────────────┼─────────────────────────────────────────────────┘
                     │ 依赖接口
                     ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     领域层 (Domain Layer)                             │
│                    DddTemplate.Domain                                │
│                   ⚠️ 零外部依赖 ⚠️                                    │
├─────────────────────────────────────────────────────────────────────┤
│  ┌────────────────────────────────────────────────────────────┐     │
│  │                    核心抽象 (Abstractions)                   │     │
│  ├────────────────────────────────────────────────────────────┤     │
│  │  Entity<TId>          │  基础实体类                         │     │
│  │  AggregateRoot<TId>   │  聚合根基类 + 领域事件管理          │     │
│  │  ValueObject          │  值对象基类                         │     │
│  │  IDomainEvent         │  领域事件接口                       │     │
│  │  DomainEvent          │  领域事件基类                       │     │
│  │  IDomainService       │  领域服务标记接口                   │     │
│  │  IRepository<T,TId>   │  仓储接口                          │     │
│  │  Result / Result<T>   │  结果模式                          │     │
│  │  Error / ErrorType    │  错误类型系统                       │     │
│  │  DomainException      │  领域异常基类                       │     │
│  └────────────────────────────────────────────────────────────┘     │
│                                                                       │
│  ┌────────────────────────────────────────────────────────────┐     │
│  │                  业务聚合 (Aggregates)                       │     │
│  ├────────────────────────────────────────────────────────────┤     │
│  │  TodoItem (聚合根)                                          │     │
│  │    ├─ Properties: Id, Title, IsCompleted, CreatedAt        │     │
│  │    ├─ Factory: Create(title)                               │     │
│  │    ├─ Behaviors: MarkCompleted(), Rename()                 │     │
│  │    └─ Events: Created, Completed, Renamed                  │     │
│  │                                                             │     │
│  │  ITodoItemRepository (仓储接口)                             │     │
│  └────────────────────────────────────────────────────────────┘     │
│                                                                       │
│  ┌────────────────────────────────────────────────────────────┐     │
│  │                  领域事件 (Domain Events)                    │     │
│  ├────────────────────────────────────────────────────────────┤     │
│  │  TodoItemCreatedEvent   (record)                            │     │
│  │  TodoItemCompletedEvent (record)                            │     │
│  │  TodoItemRenamedEvent   (record)                            │     │
│  └────────────────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────────────┘
                     ▲ 接口实现
                     │
┌─────────────────────────────────────────────────────────────────────┐
│                  基础设施层 (Infrastructure Layer)                     │
│              DddTemplate.Infrastructure.InMemory                     │
├─────────────────────────────────────────────────────────────────────┤
│  ┌──────────────────┐  ┌──────────────────┐  ┌─────────────────┐   │
│  │  InMemory        │  │   TodoItem       │  │  Dependency     │   │
│  │  Repository<T>   │  │   Repository     │  │   Injection     │   │
│  │ (ConcurrentDict) │  │                  │  │                 │   │
│  └──────────────────┘  └──────────────────┘  └─────────────────┘   │
│         │                      │                                     │
│         └──────────┬───────────┘                                     │
│                    │ 实现 IRepository<T, TId>                        │
└────────────────────┴─────────────────────────────────────────────────┘
```

## 🏗️ 分层架构详解

### 1. 表现层 (Presentation Layer) - DddTemplate.Api

**职责**: 处理 HTTP 请求、响应格式化、API 文档

**核心组件**:
```
DddTemplate.Api/
├── Program.cs                          # 应用入口、依赖注入、端点配置
├── Common/
│   └── ApiResponse.cs                  # 统一响应格式
├── Middleware/
│   └── GlobalExceptionHandlerMiddleware.cs  # 全局异常处理
└── Properties/
    └── launchSettings.json             # 启动配置
```

**关键特性**:
- ✅ Minimal API 风格（简洁、直观）
- ✅ Swagger/OpenAPI 自动文档
- ✅ 全局异常处理中间件
- ✅ 统一 ApiResponse<T> 响应格式
- ✅ 依赖注入 Application Services

**数据流**:
```
HTTP Request → Middleware Pipeline → Endpoint Handler → Application Service → Response
                    ↓
            GlobalExceptionHandler (捕获所有异常)
                    ↓
            ApiResponse<T> (统一格式)
```

---

### 2. 应用层 (Application Layer) - DddTemplate.Application

**职责**: 用例编排、业务流程协调、DTO 转换

**核心组件**:
```
DddTemplate.Application/
├── DependencyInjection.cs              # 应用层服务注册
├── Abstractions/
│   └── Messaging/                      # CQRS 抽象
│       ├── ICommand.cs                 # 命令接口
│       ├── ICommandHandler.cs          # 命令处理器接口
│       ├── IQuery.cs                   # 查询接口
│       └── IQueryHandler.cs            # 查询处理器接口
└── TodoItems/
    ├── TodoItemService.cs              # 应用服务
    ├── TodoItemDto.cs                  # 数据传输对象
    └── CreateTodoItemRequest.cs        # 请求模型
```

**关键特性**:
- ✅ CQRS 接口定义（Commands/Queries）
- ✅ Application Services 编排业务流程
- ✅ DTO 与领域模型分离
- ✅ 依赖领域层接口（IRepository）
- 🔄 待实现: MediatR 集成、FluentValidation、管道行为

**数据流**:
```
API Endpoint → Application Service → Repository → Domain Aggregate
                      ↓
                  DTO Mapping
                      ↓
                 Return Result<T>
```

---

### 3. 领域层 (Domain Layer) - DddTemplate.Domain

**职责**: 核心业务逻辑、业务规则、领域模型

**核心组件**:
```
DddTemplate.Domain/
├── Abstractions/                       # DDD 核心抽象
│   ├── Entity.cs                       # 实体基类
│   ├── AggregateRoot.cs                # 聚合根基类
│   ├── ValueObject.cs                  # 值对象基类
│   ├── IAggregateRoot.cs               # 聚合根接口
│   ├── IDomainEvent.cs                 # 领域事件接口
│   ├── DomainEvent.cs                  # 领域事件基类
│   ├── IDomainService.cs               # 领域服务接口
│   ├── IRepository.cs                  # 仓储接口
│   ├── Result.cs                       # 结果模式
│   ├── Error.cs                        # 错误类型
│   └── DomainException.cs              # 领域异常
└── TodoItems/                          # TodoItem 聚合
    ├── TodoItem.cs                     # 聚合根
    ├── ITodoItemRepository.cs          # 仓储接口
    └── Events/                         # 领域事件
        ├── TodoItemCreatedEvent.cs
        ├── TodoItemCompletedEvent.cs
        └── TodoItemRenamedEvent.cs
```

**关键特性**:
- ✅ **零外部依赖**（纯业务逻辑）
- ✅ 聚合根管理领域事件
- ✅ 封装业务规则（private setters）
- ✅ 工厂方法创建实体
- ✅ Result 模式显式错误处理
- ✅ 领域异常表达业务规则违反
- ✅ 值对象支持（ValueObject 基类）

**设计原则**:
```
1. 聚合边界清晰（TodoItem 是聚合根）
2. 不可变领域事件（record 类型）
3. 封装性（private setters + 工厂方法）
4. 显式错误处理（Result + DomainException）
5. 领域事件驱动（状态变更触发事件）
```

---

### 4. 基础设施层 (Infrastructure Layer) - DddTemplate.Infrastructure.InMemory

**职责**: 技术实现、数据持久化、外部服务集成

**核心组件**:
```
DddTemplate.Infrastructure.InMemory/
├── DependencyInjection.cs              # 基础设施服务注册
├── Common/
│   └── InMemoryRepository.cs           # 通用内存仓储
└── TodoItems/
    └── TodoItemRepository.cs           # TodoItem 仓储实现
```

**关键特性**:
- ✅ InMemory 实现（开发/测试友好）
- ✅ 线程安全（ConcurrentDictionary）
- ✅ 实现领域层仓储接口
- 🔄 可替换为 EF Core 实现

**数据流**:
```
Application Service → IRepository<T, TId> → InMemoryRepository<T, TId>
                                                    ↓
                                          ConcurrentDictionary<TId, T>
```

---

## 🔄 数据流与交互

### 创建 TodoItem 的完整流程

```
1. HTTP POST /api/todos
   Body: { "title": "学习 DDD" }
        ↓
2. Minimal API Endpoint
   app.MapPost("/api/todos", async (CreateTodoItemRequest request, ...) => { ... })
        ↓
3. Application Service
   TodoItemService.CreateAsync(request, ct)
        ↓
4. Domain Aggregate (Factory Method)
   var todoItem = TodoItem.Create(request.Title);
   // 触发领域事件: TodoItemCreatedEvent
        ↓
5. Repository
   await _repository.AddAsync(todoItem, ct);
        ↓
6. Infrastructure (InMemory)
   _storage.TryAdd(todoItem.Id, todoItem);
        ↓
7. DTO Mapping
   return new TodoItemDto(todoItem.Id, todoItem.Title, ...);
        ↓
8. API Response
   Results.Created($"/api/todos/{created.Id}", created);
        ↓
9. HTTP 201 Created
   {
     "success": true,
     "data": { "id": "...", "title": "学习 DDD", ... },
     "error": null,
     "timestamp": "2025-12-08T..."
   }
```

### 异常处理流程

```
Domain Layer: throw new ValidationException(...)
        ↓
Application Layer: 异常向上传播（或转换为 Result）
        ↓
API Layer: GlobalExceptionHandlerMiddleware 捕获
        ↓
异常类型映射:
  - ValidationException    → 400 Bad Request
  - NotFoundException      → 404 Not Found
  - ConflictException      → 409 Conflict
  - BusinessRuleException  → 422 Unprocessable Entity
  - 其他异常               → 500 Internal Server Error
        ↓
返回统一格式:
{
  "success": false,
  "data": null,
  "error": {
    "code": "TodoItem.InvalidTitle",
    "message": "标题不能为空",
    "type": "Validation"
  },
  "timestamp": "2025-12-08T..."
}
```

---

## 🎯 核心设计模式

### 1. 聚合根模式 (Aggregate Root Pattern)

```csharp
// 聚合根是一组相关对象的集合，作为数据修改的单元
public sealed class TodoItem : AggregateRoot<Guid>
{
    // 封装状态（private setters）
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }

    // 工厂方法（唯一创建方式）
    public static TodoItem Create(string title)
    {
        var item = new TodoItem(Guid.NewGuid(), title);
        item.AddDomainEvent(new TodoItemCreatedEvent(...));  // 发布事件
        return item;
    }

    // 业务行为（封装业务规则）
    public void MarkCompleted()
    {
        if (IsCompleted) return;  // 幂等性
        IsCompleted = true;
        AddDomainEvent(new TodoItemCompletedEvent(Id));
    }
}
```

**关键点**:
- ✅ 聚合边界清晰
- ✅ 状态变更通过方法
- ✅ 领域事件记录变更
- ✅ 业务规则封装在聚合内

---

### 2. 领域事件模式 (Domain Events Pattern)

```csharp
// 事件定义（不可变 record）
public sealed record TodoItemCreatedEvent : DomainEvent
{
    public Guid TodoItemId { get; init; }
    public string Title { get; init; }
}

// 事件管理（在 AggregateRoot 中）
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
```

**事件流程**:
```
1. 业务方法执行 → AddDomainEvent()
2. Repository 保存聚合
3. Application 层获取事件: aggregate.DomainEvents
4. 发布事件到处理器（待实现 MediatR）
5. 清除事件: aggregate.ClearDomainEvents()
```

---

### 3. Result 模式 (Result Pattern)

```csharp
// 显式表达成功/失败
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

// 泛型版本（携带数据）
public class Result<TValue> : Result
{
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException();
}

// 使用示例
public async Task<Result<TodoItem>> GetAsync(Guid id)
{
    var item = await _repository.GetByIdAsync(id);
    return item is not null
        ? Result.Success(item)
        : Result.Failure<TodoItem>(Error.NotFound("TodoItem.NotFound", "未找到"));
}
```

**优势**:
- ✅ 类型安全的错误处理
- ✅ 强制调用者处理错误
- ✅ 避免异常性能开销
- ✅ 更好的错误追踪

---

### 4. 仓储模式 (Repository Pattern)

```csharp
// 领域层定义接口
public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task RemoveAsync(TEntity entity, CancellationToken ct = default);
}

// 基础设施层实现
public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
{
    private readonly ConcurrentDictionary<TId, TEntity> _storage = new();

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
        => Task.FromResult(_storage.TryGetValue(id, out var entity) ? entity : null);
}
```

**关键点**:
- ✅ 接口在领域层（依赖倒置）
- ✅ 实现在基础设施层
- ✅ 聚合根是仓储的操作单元
- ✅ 可替换实现（InMemory → EF Core）

---

### 5. CQRS 模式 (Command Query Responsibility Segregation)

```csharp
// 命令（修改状态）
public interface ICommand { }
public interface ICommand<out TResponse> : ICommand { }

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken ct);
}

// 查询（读取数据）
public interface IQuery<out TResponse> { }

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken ct);
}
```

**状态**: 接口已定义，待 MediatR 集成后完整实现

---

## 📦 依赖关系图

```
┌─────────────────────────────────────────────────────────────┐
│                      依赖方向（单向）                          │
└─────────────────────────────────────────────────────────────┘

Api ──────────────────────┐
                          ├──→ Application ──→ Domain (核心)
Infrastructure.InMemory ──┘                      ↑
                                                 │
                                          零外部依赖
```

**依赖规则**:
1. ✅ Domain 层：零依赖（只依赖 .NET 基础库）
2. ✅ Application 层：只依赖 Domain
3. ✅ Infrastructure 层：依赖 Domain（实现接口）
4. ✅ Api 层：依赖 Application 和 Infrastructure（组装）

---

## 🔧 技术栈

| 层级 | 技术 | 版本 | 用途 |
|------|------|------|------|
| **运行时** | .NET | 9.0 | 应用平台 |
| **语言** | C# | 13 | 编程语言 |
| **API** | ASP.NET Core Minimal API | 9.0 | Web API |
| **文档** | Swashbuckle.AspNetCore | 10.0.1 | Swagger/OpenAPI |
| **中介者** | MediatR | 14.0.0 | CQRS 实现（已添加） |
| **验证** | FluentValidation | - | 输入验证（待添加） |
| **持久化** | InMemory | - | 开发/测试 |
| **持久化** | EF Core | - | 生产环境（待添加） |

---

## ✅ 已实现功能清单

### 领域层 (Domain)
- ✅ Entity<TId> 基类
- ✅ AggregateRoot<TId> 基类
- ✅ ValueObject 基类
- ✅ IAggregateRoot 接口
- ✅ IDomainEvent / DomainEvent
- ✅ IDomainService 接口
- ✅ IRepository<T, TId> 接口
- ✅ Result / Result<T> 模式
- ✅ Error / ErrorType 系统
- ✅ DomainException 及 4 个子类
- ✅ TodoItem 聚合根
- ✅ 3 个领域事件（Created, Completed, Renamed）

### 应用层 (Application)
- ✅ TodoItemService 应用服务
- ✅ TodoItemDto 数据传输对象
- ✅ CQRS 接口（ICommand, IQuery, Handlers）
- ✅ 依赖注入配置

### 基础设施层 (Infrastructure)
- ✅ InMemoryRepository<T, TId> 通用实现
- ✅ TodoItemRepository 具体实现
- ✅ 线程安全（ConcurrentDictionary）

### 表现层 (Api)
- ✅ Minimal API 端点（5 个）
- ✅ GlobalExceptionHandlerMiddleware
- ✅ ApiResponse<T> 统一响应
- ✅ Swagger 集成
- ✅ 依赖注入配置

### 文档
- ✅ CLAUDE.md（完整使用指南）
- ✅ README.md（项目概览）
- ✅ PHASE2_IMPLEMENTATION_PLAN.md（第二阶段计划）
- ✅ ARCHITECTURE.md（本文档）

---

## 🔄 待实现功能（第二阶段）

### 高优先级
1. ⏳ MediatR 完整集成
   - 更新 CQRS 接口继承 IRequest
   - 创建具体 Commands/Queries
   - 创建对应 Handlers
   - 配置 MediatR 管道

2. ⏳ FluentValidation 集成
   - 添加 NuGet 包
   - 创建验证器
   - 创建验证管道行为

3. ⏳ 管道行为 (Pipeline Behaviors)
   - ValidationBehavior
   - LoggingBehavior
   - TransactionBehavior

### 中优先级
4. ⏳ 领域事件发布
   - IDomainEventPublisher 接口
   - MediatR 事件发布器实现
   - 在仓储保存后发布

5. ⏳ 工作单元模式
   - IUnitOfWork 接口
   - 事务管理

### 低优先级
6. ⏳ EF Core 持久化
   - DbContext 配置
   - 实体配置（Fluent API）
   - 数据库迁移

7. ⏳ 规约模式 (Specification Pattern)
   - 复杂查询封装

---

## 🎓 设计原则总结

### SOLID 原则应用

1. **单一职责原则 (SRP)**
   - 每个聚合负责一个业务概念
   - 每个层级有明确职责

2. **开闭原则 (OCP)**
   - 通过接口扩展（IRepository）
   - 管道行为可插拔

3. **里氏替换原则 (LSP)**
   - InMemory → EF Core 无缝替换
   - 所有实现遵循接口契约

4. **接口隔离原则 (ISP)**
   - 小而专注的接口（IDomainEvent, IDomainService）
   - CQRS 分离读写

5. **依赖倒置原则 (DIP)**
   - 高层模块（Application）依赖抽象（IRepository）
   - 低层模块（Infrastructure）实现抽象

### DDD 战术模式

- ✅ Entity（实体）
- ✅ Value Object（值对象）
- ✅ Aggregate Root（聚合根）
- ✅ Domain Event（领域事件）
- ✅ Repository（仓储）
- ✅ Domain Service（领域服务）
- ⏳ Specification（规约）
- ⏳ Factory（工厂）

---

## 📊 代码统计

```
总文件数: ~30 个
总代码行数: ~2000 行

分层占比:
- Domain:         40% (核心业务逻辑)
- Application:    20% (用例编排)
- Infrastructure: 15% (技术实现)
- Api:            15% (API 端点)
- Documentation:  10% (文档)
```

---

## 🚀 快速开始

```bash
# 1. 恢复依赖
dotnet restore

# 2. 构建项目
dotnet build

# 3. 运行 API
dotnet run --project src/DddTemplate.Api/DddTemplate.Api.csproj

# 4. 访问 Swagger
# http://localhost:5002/swagger
```

---

## 📚 参考资源

- [Domain-Driven Design (Eric Evans)](https://www.domainlanguage.com/ddd/)
- [Clean Architecture (Robert C. Martin)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern (Martin Fowler)](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)

---

**文档版本**: 1.0
**最后更新**: 2025-12-08
**维护者**: DDD Template Team
