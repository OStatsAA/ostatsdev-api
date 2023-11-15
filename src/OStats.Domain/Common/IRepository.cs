namespace OStats.Domain.Common;

public interface IRepository<T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
    T Add(T entity);
    T Update(T entity);
    void Delete(T entity);
    Task<T?> FindByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}