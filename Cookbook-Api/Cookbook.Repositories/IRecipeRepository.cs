using Cookbook.Domain.Recipe;

namespace Cookbook.Repositories;

public interface IRecipeRepository
{
    Task Add(Recipe recipe);

    Task<Recipe> GetById(int id);

    Task<IEnumerable<Recipe>> GetAll();
}