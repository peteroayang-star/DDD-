# 同时启动 API 和 Admin 项目的脚本

Write-Host "正在启动 DDD Template 项目..." -ForegroundColor Green
Write-Host ""

# 启动 API 项目（后台运行）
Write-Host "1. 启动 API 项目 (http://localhost:5002)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'D:\创新项目\DDD 基础架构\src\DddTemplate.Api'; dotnet run"

# 等待 3 秒让 API 启动
Write-Host "   等待 API 启动..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# 启动 Admin 项目（后台运行）
Write-Host "2. 启动 Admin 项目 (http://localhost:5001)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'D:\创新项目\DDD 基础架构\DddTemplate.Admin'; dotnet run"

Write-Host ""
Write-Host "✓ 两个项目已启动！" -ForegroundColor Green
Write-Host ""
Write-Host "访问地址：" -ForegroundColor Yellow
Write-Host "  - API:   http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  - Admin: http://localhost:5001" -ForegroundColor White
Write-Host ""
Write-Host "按任意键关闭此窗口..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
