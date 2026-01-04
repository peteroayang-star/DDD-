# API 启动脚本 - 自动清理端口并启动服务

Write-Host "正在检查端口 5002 占用情况..." -ForegroundColor Yellow

# 查找占用 5002 端口的进程
$port = 5002
$connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue

if ($connections) {
    Write-Host "发现端口 $port 被占用,正在清理..." -ForegroundColor Yellow

    foreach ($conn in $connections) {
        $processId = $conn.OwningProcess
        $process = Get-Process -Id $processId -ErrorAction SilentlyContinue

        if ($process) {
            Write-Host "  - 停止进程: $($process.ProcessName) (PID: $processId)" -ForegroundColor Red
            Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
        }
    }

    # 等待端口释放
    Start-Sleep -Seconds 2
    Write-Host "端口已清理" -ForegroundColor Green
} else {
    Write-Host "端口 $port 未被占用" -ForegroundColor Green
}

# 启动 API 服务器
Write-Host "`n正在启动 API 服务器..." -ForegroundColor Cyan
Set-Location "D:\创新项目\DDD 基础架构"
dotnet run --project src/DddTemplate.Api/DddTemplate.Api.csproj
