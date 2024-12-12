using Cookbook.Domain.Recipe;

namespace Cookbook.Repositories;

public interface IRecipeRepository
{
    Task Add(Recipe recipe, CancellationToken cancellationToken = default);

    Task<Recipe> GetById(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Recipe>> GetAll(CancellationToken cancellationToken = default);
}