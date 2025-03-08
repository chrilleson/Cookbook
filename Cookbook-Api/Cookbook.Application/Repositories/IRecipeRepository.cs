namespace Cookbook.Application.Repositories;

public interface IRecipeRepository
{
    Task Add(Domain.Recipe.Entities.Recipe recipe, CancellationToken cancellationToken = default);

    Task<bool> AnyById(int id, CancellationToken cancellationToken = default);

    Task<Domain.Recipe.Entities.Recipe?> GetById(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Domain.Recipe.Entities.Recipe>> GetAll(CancellationToken cancellationToken = default);

    void Update(Domain.Recipe.Entities.Recipe recipe);

    void Remove(Domain.Recipe.Entities.Recipe recipe);
}
