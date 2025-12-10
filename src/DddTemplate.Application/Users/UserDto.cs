namespace DddTemplate.Application.Users;

/// <summary>
/// User 数据传输对象
/// </summary>
public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);
