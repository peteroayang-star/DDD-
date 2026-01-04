# API 停止脚本 - 清理占用 5002 端口的进程

Write-Host "正在查找占用端口 5002 的进程..." -ForegroundColor Yellow

$port = 5002
$connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue

if ($connections) {
    $processIds = $connections | Select-Object -ExpandProperty OwningProcess -Unique

    foreach ($pid in $processIds) {
        $process = Get-Process -Id $pid -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "停止进程: $($process.ProcessName) (PID: $pid)" -ForegroundColor Red
            Stop-Process -Id $pid -Force
        }
    }

    Write-Host "端口 5002 已释放" -ForegroundColor Green
} else {
    Write-Host "端口 5002 未被占用" -ForegroundColor Green
}
