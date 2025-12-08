using DddTemplate.Application;
using DddTemplate.Application.TodoItems;
using DddTemplate.Infrastructure.InMemory;
using DddTemplate.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. 注册各层服务
builder.Services.AddApplication();
builder.Services.AddInMemoryInfrastructure();

// 2. 添加基础设施服务（日志、Swagger等）
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. 配置中间件管道
// 全局异常处理（必须在最前面）
app.UseGlobalExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 4. 配置 API 端点（直接调用 Application Service）

// 获取所有待办事项
app.MapGet("/api/todos", async (TodoItemService service, CancellationToken ct) =>
{
    var result = await service.ListAsync(ct);
    return Results.Ok(result);
})
.WithName("GetTodos")
.WithTags("Todos");

// 根据 ID 获取待办事项
app.MapGet("/api/todos/{id:guid}", async (Guid id, TodoItemService service, CancellationToken ct) =>
{
    var item = await service.GetAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
})
.WithName("GetTodoById")
.WithTags("Todos");

// 创建新的待办事项
app.MapPost("/api/todos", async (CreateTodoItemRequest request, TodoItemService service, CancellationToken ct) =>
{
    var created = await service.CreateAsync(request, ct);
    return Results.Created($"/api/todos/{created.Id}", created);
})
.WithName("CreateTodo")
.WithTags("Todos");

// 标记待办事项为完成
app.MapPut("/api/todos/{id:guid}/complete", async (Guid id, TodoItemService service, CancellationToken ct) =>
{
    var ok = await service.CompleteAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
})
.WithName("CompleteTodo")
.WithTags("Todos");

// 重命名待办事项
app.MapPut("/api/todos/{id:guid}/rename", async (Guid id, string title, TodoItemService service, CancellationToken ct) =>
{
    var ok = await service.RenameAsync(id, title, ct);
    return ok ? Results.NoContent() : Results.NotFound();
})
.WithName("RenameTodo")
.WithTags("Todos");

app.Run();
