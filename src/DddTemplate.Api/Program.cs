using DddTemplate.Application;
using DddTemplate.Application.TodoItems;
using DddTemplate.Infrastructure.InMemory;
using DddTemplate.Api.Middleware;
using Serilog;

// 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .CreateLogger();

try
{
    Log.Information("========================================");
    Log.Information("Starting DDD Template API...");
    Log.Information("Environment: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");
    Log.Information("========================================");

    var builder = WebApplication.CreateBuilder(args);

    // 使用 Serilog 替换默认日志
    builder.Host.UseSerilog();

    // 1. 注册各层服务
    builder.Services.AddApplication();
    builder.Services.AddInMemoryInfrastructure();

    // 2. 添加基础设施服务（日志、Swagger、Controllers等）
    builder.Services.AddControllers(); // 添加Controllers支持
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // 3. 配置中间件管道
    // 全局异常处理（必须在最前面）
    app.UseGlobalExceptionHandler();

    // Serilog HTTP 请求日志记录
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    // 4. 配置 API 端点（直接调用 Application Service）

    // 获取所有待办事项
    app.MapGet("/api/todos", async (TodoItemService service, ILogger<Program> logger, CancellationToken ct) =>
    {
        logger.LogInformation("Fetching all todo items");
        var result = await service.ListAsync(ct);
        logger.LogInformation("Retrieved {Count} todo items", result.Count);
        return Results.Ok(result);
    })
    .WithName("GetTodos")
    .WithTags("Todos");

    // 根据 ID 获取待办事项
    app.MapGet("/api/todos/{id:guid}", async (Guid id, TodoItemService service, ILogger<Program> logger, CancellationToken ct) =>
    {
        logger.LogInformation("Fetching todo item with ID: {TodoId}", id);
        var item = await service.GetAsync(id, ct);

        if (item is null)
        {
            logger.LogWarning("Todo item with ID {TodoId} not found", id);
            return Results.NotFound();
        }

        logger.LogInformation("Retrieved todo item: {TodoId} - {Title}", item.Id, item.Title);
        return Results.Ok(item);
    })
    .WithName("GetTodoById")
    .WithTags("Todos");

    // 创建新的待办事项
    app.MapPost("/api/todos", async (CreateTodoItemRequest request, TodoItemService service, ILogger<Program> logger, CancellationToken ct) =>
    {
        logger.LogInformation("Creating new todo item with title: {Title}", request.Title);
        var created = await service.CreateAsync(request, ct);
        logger.LogInformation("Created todo item with ID: {TodoId}", created.Id);
        return Results.Created($"/api/todos/{created.Id}", created);
    })
    .WithName("CreateTodo")
    .WithTags("Todos");

    // 标记待办事项为完成
    app.MapPut("/api/todos/{id:guid}/complete", async (Guid id, TodoItemService service, ILogger<Program> logger, CancellationToken ct) =>
    {
        logger.LogInformation("Marking todo item {TodoId} as completed", id);
        var ok = await service.CompleteAsync(id, ct);

        if (!ok)
        {
            logger.LogWarning("Failed to complete todo item {TodoId} - not found", id);
            return Results.NotFound();
        }

        logger.LogInformation("Todo item {TodoId} marked as completed", id);
        return Results.NoContent();
    })
    .WithName("CompleteTodo")
    .WithTags("Todos");

    // 重命名待办事项
    app.MapPut("/api/todos/{id:guid}/rename", async (Guid id, string title, TodoItemService service, ILogger<Program> logger, CancellationToken ct) =>
    {
        logger.LogInformation("Renaming todo item {TodoId} to: {NewTitle}", id, title);
        var ok = await service.RenameAsync(id, title, ct);

        if (!ok)
        {
            logger.LogWarning("Failed to rename todo item {TodoId} - not found", id);
            return Results.NotFound();
        }

        logger.LogInformation("Todo item {TodoId} renamed successfully", id);
        return Results.NoContent();
    })
    .WithName("RenameTodo")
    .WithTags("Todos");

    // 5. 映射Controllers
    app.MapControllers();

    Log.Information("========================================");
    Log.Information("Application configured successfully");
    Log.Information("Starting web host on: {Urls}", string.Join(", ", app.Urls));
    Log.Information("========================================");

    app.Run();

    Log.Information("Application stopped gracefully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.Information("Shutting down Serilog...");
    Log.CloseAndFlush();
}
