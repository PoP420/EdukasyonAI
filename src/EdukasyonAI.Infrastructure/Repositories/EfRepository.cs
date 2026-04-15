using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Repositories;
using EdukasyonAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Repositories;

/// <summary>
/// Generic EF Core repository implementation.
/// </summary>
public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly EdukasyonDbContext _db;
    protected readonly DbSet<TEntity> _set;

    public EfRepository(EdukasyonDbContext db)
    {
        _db = db;
        _set = db.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _set.FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _set.ToListAsync(cancellationToken);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
