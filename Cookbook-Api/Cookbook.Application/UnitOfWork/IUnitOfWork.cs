namespace Cookbook.Application.UnitOfWork;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    void Rollback();
}