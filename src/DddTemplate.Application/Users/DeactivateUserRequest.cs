namespace DddTemplate.Application.Users;

/// <summary>
/// 停用用户请求
/// </summary>
public sealed record DeactivateUserRequest(
    string Reason
);
