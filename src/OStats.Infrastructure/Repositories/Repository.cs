using Microsoft.EntityFrameworkCore;
using OStats.Domain.Common;

namespace OStats.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : Entity, IAggregateRoot
{
    protected readonly Context _context;
    public IUnitOfWork UnitOfWork => _context;

    public Repository(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public T Add(T entity)
    {
        return _context.Set<T>()
                       .Add(entity).Entity;
    }

    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await _context.Set<T>()
                             .Where(entity => entity.Id == id)
                             .SingleOrDefaultAsync();
    }

    public T Update(T entity)
    {
        return _context.Set<T>()
                       .Update(entity).Entity;
    }

    public void Delete(T entity)
    {
        _context.Set<T>()
                .Remove(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Set<T>()
                             .Where(entity => entity.Id == id)
                             .AnyAsync();
    }
}