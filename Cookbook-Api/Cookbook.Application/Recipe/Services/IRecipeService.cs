using Cookbook.Application.Recipe.Models;

namespace Cookbook.Application.Recipe.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RecipeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RecipeDto> CreateAsync(CreateRecipeDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateRecipeDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
