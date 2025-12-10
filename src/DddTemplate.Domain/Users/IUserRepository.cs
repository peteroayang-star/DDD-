using DddTemplate.Domain.Abstractions;
using DddTemplate.Domain.Users.ValueObjects;

namespace DddTemplate.Domain.Users;

/// <summary>
/// User 仓储接口
/// 演示如何扩展基础仓储接口，添加特定查询方法
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// 根据邮箱查找用户
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    Task<bool> ExistsWithEmailAsync(Email email, CancellationToken ct = default);

    /// <summary>
    /// 获取活跃用户列表
    /// </summary>
    Task<IReadOnlyList<User>> GetActiveUsersAsync(CancellationToken ct = default);
}
