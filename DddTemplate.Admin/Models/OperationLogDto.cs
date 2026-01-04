namespace DddTemplate.Admin.Models;

/// <summary>
/// 操作日志数据传输对象
/// </summary>
public class OperationLogDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestParams { get; set; }
    public string? IpAddress { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime OperatedAt { get; set; }
    public long ExecutionTime { get; set; }
}
