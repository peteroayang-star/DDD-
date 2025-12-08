# DDD 基础架构模板

一个基于 .NET 9.0 的领域驱动设计（DDD）基础架构模板，实现了核心 DDD 战术模式和现代错误处理机制。

## 🎯 项目特性

### ✅ 已实现的核心功能

#### 1. DDD 战术模式
- **Entity 基类**: 基于 ID 的实体相等性
- **AggregateRoot 基类**: 支持领域事件管理
- **ValueObject 基类**: 基于值的相等性
- **Repository 模式**: 通用仓储接口和内存实现
- **Domain Service**: 领域服务标记接口
- **Domain Events**: 领域事件系统（3个示例事件）

#### 2. 错误处理系统
- **Result 模式**: 优雅的错误处理，替代异常
- **Error 类型系统**: 6种错误类型（Validation, NotFound, Conflict, Failure, Unauthorized, Forbidden）
- **自定义领域异常**: 4种领域异常类型
- **全局异常处理**: 统一的异常处理中间件
- **统一 API 响应**: 标准化的响应格式

#### 3. CQRS 基础
- **ICommand/IQuery**: 命令查询分离接口
- **ICommandHandler/IQueryHandler**: 处理器接口
- **MediatR 集成准备**: 已添加 MediatR 14.0.0

#### 4. 基础设施
- **InMemory Repository**: 线程安全的内存仓储实现
- **依赖注入**: 分层 DI 配置
- **Swagger 集成**: 自动 API 文档

## 🏗️ 项目结构

```
DddTemplate/
├── src/
│   ├── DddTemplate.Domain/              # 领域层（无依赖）
│   │   ├── Abstractions/                # DDD 核心抽象
│   │   │   ├── Entity.cs
│   │   │   ├── AggregateRoot.cs
│   │   │   ├── ValueObject.cs
│   │   │   ├── DomainEvent.cs
│   │   │   ├── Result.cs
│   │   │   ├── Error.cs
│   │   │   └── DomainException.cs
│   │   └── TodoItems/                   # 业务聚合
│   │       ├── TodoItem.cs
│   │       └── Events/
│   │
│   ├── DddTemplate.Application/         # 应用层
│   │   ├── Abstractions/
│   │   │   └── Messaging/               # CQRS 抽象
│   │   └── TodoItems/
│   │       ├── TodoItemService.cs
│   │       └── TodoItemDto.cs
│   │
│   ├── DddTemplate.Infrastructure.InMemory/  # 基础设施层
│   │   ├── Common/
│   │   │   └── InMemoryRepository.cs
│   │   └── TodoItems/
│   │
│   └── DddTemplate.Api/                 # 表现层
│       ├── Program.cs
│       ├── Common/
│       │   └── ApiResponse.cs
│       └── Middleware/
│           └── GlobalExceptionHandlerMiddleware.cs
│
├── CLAUDE.md                            # Claude Code 使用指南
├── PHASE2_IMPLEMENTATION_PLAN.md       # 第二阶段实施计划
└── README.md                            # 本文件
```

## 🚀 快速开始

### 前置要求

- .NET 9.0 SDK
- Visual Studio 2022 或 VS Code

### 运行项目

```bash
# 克隆或下载项目
cd "D:\创作\DDD 基础架构"

# 恢复依赖
dotnet restore

# 构建项目
dotnet build

# 运行 API
dotnet run --project src/DddTemplate.Api/DddTemplate.Api.csproj
```

### 访问 API

- **API 地址**: http://localhost:5002
- **Swagger UI**: http://localhost:5002/swagger

### API 端点

```
GET    /api/todos           # 获取所有待办事项
GET    /api/todos/{id}      # 获取单个待办事项
POST   /api/todos           # 创建待办事项
PUT    /api/todos/{id}/complete    # 标记为完成
PUT    /api/todos/{id}/rename      # 重命名
```

## 📖 使用示例

### 1. 创建聚合根

```csharp
public sealed class TodoItem : AggregateRoot<Guid>
{
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }

    public static TodoItem Create(string title)
    {
        var item = new TodoItem(Guid.NewGuid(), title);
        item.AddDomainEvent(new TodoItemCreatedEvent(item.Id, title));
        return item;
    }

    public void MarkCompleted()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        AddDomainEvent(new TodoItemCompletedEvent(Id));
    }
}
```

### 2. 使用 Result 模式

```csharp
public async Task<Result<TodoItem>> GetTodoItem(Guid id)
{
    var item = await _repository.GetByIdAsync(id);

    return item is not null
        ? Result.Success(item)
        : Result.Failure<TodoItem>(
            Error.NotFound("TodoItem.NotFound", "TodoItem not found")
        );
}
```

### 3. 抛出领域异常

```csharp
if (string.IsNullOrEmpty(title))
    throw new ValidationException(
        "TodoItem.InvalidTitle",
        "Title cannot be empty"
    );
```

### 4. 统一 API 响应

```csharp
// 成功响应
{
  "success": true,
  "data": { "id": "...", "title": "..." },
  "error": null,
  "timestamp": "2025-12-08T07:20:00Z"
}

// 错误响应
{
  "success": false,
  "data": null,
  "error": {
    "code": "TodoItem.NotFound",
    "message": "TodoItem not found",
    "type": "NotFound"
  },
  "timestamp": "2025-12-08T07:20:00Z"
}
```

## 🎓 核心概念

### 聚合根 (Aggregate Root)

聚合根是一组相关对象的集合，作为数据修改的单元。只有聚合根可以被仓储直接访问。

**特性**:
- 维护聚合边界
- 管理领域事件
- 封装业务规则

### 领域事件 (Domain Events)

领域事件表示领域中发生的重要业务事件。

**工作流程**:
1. 业务方法执行 → 添加事件到聚合
2. 仓储保存聚合
3. 应用层发布事件
4. 事件处理器响应

### Result 模式

Result 模式提供了一种优雅的错误处理方式，避免了异常的性能开销。

**优势**:
- 显式表达成功/失败
- 类型安全
- 更好的错误追踪
- 支持链式调用

### CQRS 模式

命令查询职责分离（CQRS）将读操作和写操作分离。

**组件**:
- **Command**: 修改系统状态
- **Query**: 读取数据，不修改状态
- **Handler**: 处理 Command 或 Query

## 📚 文档

- **[CLAUDE.md](./CLAUDE.md)**: 完整的使用指南和架构说明
- **[PHASE2_IMPLEMENTATION_PLAN.md](./PHASE2_IMPLEMENTATION_PLAN.md)**: 第二阶段实施计划（MediatR + FluentValidation + CQRS 完整实现）

## 🔄 下一步

### 第二阶段功能（计划中）

1. **MediatR 完整集成**
   - 更新接口以继承 MediatR.IRequest
   - 创建具体的 Commands 和 Queries
   - 创建对应的 Handlers

2. **FluentValidation**
   - 添加验证器
   - 创建验证管道行为

3. **管道行为**
   - 验证管道
   - 日志管道
   - 事务管道

4. **工作单元模式**
   - IUnitOfWork 接口
   - 事务管理

5. **EF Core 持久化**
   - DbContext 配置
   - 实体配置
   - 数据库迁移

详细实施步骤请参考 [PHASE2_IMPLEMENTATION_PLAN.md](./PHASE2_IMPLEMENTATION_PLAN.md)。

## 🛠️ 技术栈

- **.NET 9.0**: 最新的 .NET 平台
- **C# 13**: 最新的 C# 语言特性
- **ASP.NET Core**: Web API 框架
- **Minimal API**: 简洁的 API 定义方式
- **Swagger/OpenAPI**: API 文档
- **MediatR**: 中介者模式实现（已添加）

## 📝 设计原则

1. **依赖倒置**: 领域层无外部依赖
2. **单向依赖**: 严格的层级依赖规则
3. **封装**: 使用 private setters 和工厂方法
4. **不可变性**: 领域事件使用 record 类型
5. **显式错误处理**: Result 模式 + 领域异常

## 🤝 贡献

这是一个模板项目，可以根据实际需求进行定制和扩展。

## 📄 许可

MIT License

## 🙏 致谢

本项目基于 DDD 战术模式和 Clean Architecture 原则构建。

---

**开始使用**: 查看 [CLAUDE.md](./CLAUDE.md) 获取详细的使用指南！
