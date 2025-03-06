namespace Cookbook.Application.Repositories;

public interface IRecipeRepository
{
    Task Add(Domain.Recipe.Recipe recipe, CancellationToken cancellationToken = default);

    Task<bool> AnyById(int id, CancellationToken cancellationToken = default);

    Task<Domain.Recipe.Recipe?> GetById(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Domain.Recipe.Recipe>> GetAll(CancellationToken cancellationToken = default);

    void Update(Domain.Recipe.Recipe recipe);

    void Remove(Domain.Recipe.Recipe recipe);
}