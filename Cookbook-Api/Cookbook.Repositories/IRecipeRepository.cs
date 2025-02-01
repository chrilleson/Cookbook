using Cookbook.Domain.Recipe;

namespace Cookbook.Repositories;

public interface IRecipeRepository
{
    Task Add(Recipe recipe, CancellationToken cancellationToken = default);

    Task<bool> AnyById(int id, CancellationToken cancellationToken = default);

    Task<Recipe?> GetById(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Recipe>> GetAll(CancellationToken cancellationToken = default);

    void Update(Recipe recipe);

    void Remove(Recipe recipe);
}