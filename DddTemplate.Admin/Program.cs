var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register API services
builder.Services.AddScoped<DddTemplate.Admin.Services.TodoItemApiService>();
builder.Services.AddScoped<DddTemplate.Admin.Services.DashboardApiService>();
builder.Services.AddScoped<DddTemplate.Admin.Services.OperationLogApiService>();
builder.Services.AddScoped<DddTemplate.Admin.Services.MenuApiService>();

// Register visit statistics service as singleton
builder.Services.AddSingleton<DddTemplate.Admin.Services.VisitStatisticsService>();
builder.Services.AddSingleton<DddTemplate.Admin.Services.UserStatisticsService>();
builder.Services.AddSingleton<DddTemplate.Admin.Services.SystemMessageService>();

// Add HttpClient for API calls
builder.Services.AddHttpClient("DddTemplateApi", client =>
{
    // 使用 HTTP 连接到 API（开发环境）
    client.BaseAddress = new Uri("http://localhost:5000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 暂时禁用认证中间件，方便开发测试
// app.UseMiddleware<DddTemplate.Admin.Middleware.AuthenticationMiddleware>();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TodoItem}/{action=Index}/{id?}");

app.Run();
