Write-Host "=== 测试仪表盘数据 ===" -ForegroundColor Green
Write-Host ""
Write-Host "请确保已启动以下服务：" -ForegroundColor Yellow
Write-Host "1. API 服务: dotnet run --project src/DddTemplate.Api/DddTemplate.Api.csproj"
Write-Host "2. Admin 服务: dotnet run --project DddTemplate.Admin/DddTemplate.Admin.csproj"
Write-Host ""
Write-Host "测试步骤：" -ForegroundColor Cyan
Write-Host "1. 访问 http://localhost:5000"
Write-Host "2. 使用 admin/admin123 登录"
Write-Host "3. 查看仪表盘数据"
Write-Host ""
Write-Host "预期结果：" -ForegroundColor Green
Write-Host "- 待办事项数：根据实际数据显示"
Write-Host "- 今日访问数：1（首次登录）或更多（多次登录）"
