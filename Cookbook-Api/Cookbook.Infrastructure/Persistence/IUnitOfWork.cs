namespace Cookbook.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    void Rollback();
}