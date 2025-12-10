namespace DddTemplate.Application.Users;

/// <summary>
/// 创建用户请求
/// </summary>
public sealed record CreateUserRequest(
    string Email,
    string FullName
);
