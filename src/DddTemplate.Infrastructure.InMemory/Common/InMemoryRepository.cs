using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using DddTemplate.Domain.Abstractions;

namespace DddTemplate.Infrastructure.InMemory.Common;

public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
{
    protected readonly ConcurrentDictionary<TId, TEntity> Store = new();

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default)
    {
        IReadOnlyList<TEntity> list = Store.Values.ToList();
        return Task.FromResult(list);
    }

    public Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        Store[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        Store.TryRemove(entity.Id, out _);
        return Task.CompletedTask;
    }
}

