namespace DddTemplate.Application.Users;

/// <summary>
/// 更新用户请求
/// </summary>
public sealed record UpdateUserRequest(
    string? Email,
    string? FullName
);
