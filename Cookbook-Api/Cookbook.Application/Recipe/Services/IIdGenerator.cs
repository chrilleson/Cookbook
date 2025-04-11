namespace Cookbook.Application.Recipe.Services;

public interface IIdGenerator
{
    Task<int> GenerateNextId(CancellationToken cancellationToken = default);
}
