using DddTemplate.Domain.Users;
using DddTemplate.Domain.Users.ValueObjects;
using DddTemplate.Infrastructure.InMemory.Common;

namespace DddTemplate.Infrastructure.InMemory.Users;

/// <summary>
/// User 内存仓储实现
/// 演示如何扩展基础仓储，实现特定查询方法
/// </summary>
public sealed class InMemoryUserRepository : InMemoryRepository<User, Guid>, IUserRepository
{
    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
    {
        var user = Store.Values.FirstOrDefault(u => u.Email.Equals(email));
        return Task.FromResult(user);
    }

    public Task<bool> ExistsWithEmailAsync(Email email, CancellationToken ct = default)
    {
        var exists = Store.Values.Any(u => u.Email.Equals(email));
        return Task.FromResult(exists);
    }

    public Task<IReadOnlyList<User>> GetActiveUsersAsync(CancellationToken ct = default)
    {
        IReadOnlyList<User> users = Store.Values
            .Where(u => u.IsActive)
            .OrderBy(u => u.CreatedAt)
            .ToList();
        return Task.FromResult(users);
    }
}
