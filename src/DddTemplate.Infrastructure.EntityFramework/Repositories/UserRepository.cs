using Microsoft.EntityFrameworkCore;
using DddTemplate.Domain.Users;
using DddTemplate.Domain.Users.ValueObjects;

namespace DddTemplate.Infrastructure.EntityFramework.Repositories;

public class UserRepository : EfRepository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<bool> ExistsWithEmailAsync(Email email, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, ct);
    }

    public async Task<IReadOnlyList<User>> GetActiveUsersAsync(CancellationToken ct = default)
    {
        return await _context.Users.Where(u => u.IsActive).ToListAsync(ct);
    }
}
