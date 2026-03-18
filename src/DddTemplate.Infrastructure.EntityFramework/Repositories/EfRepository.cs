using Microsoft.EntityFrameworkCore;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Infrastructure.EntityFramework.Repositories;

public class EfRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
{
    protected readonly ApplicationDbContext _context;

    public EfRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        return await _context.Set<TEntity>().FindAsync(new object[] { id }, ct);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default)
    {
        return await _context.Set<TEntity>().ToListAsync(ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _context.Set<TEntity>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}
