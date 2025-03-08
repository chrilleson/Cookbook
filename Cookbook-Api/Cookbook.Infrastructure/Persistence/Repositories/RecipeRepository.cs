using Cookbook.Application.Repositories;
using Cookbook.Domain.Recipe.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Persistence.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _dbContext;

    public RecipeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Recipe recipe, CancellationToken cancellationToken = default) =>
        await _dbContext.Recipes.AddAsync(recipe, cancellationToken);

    public async Task<bool> AnyById(int id, CancellationToken cancellationToken = default) =>
        await _dbContext.Recipes.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<Recipe?> GetById(int id, CancellationToken cancellationToken = default) =>
        await _dbContext.Recipes.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<Recipe>> GetAll(CancellationToken cancellationToken = default) =>
        await _dbContext.Recipes.ToListAsync(cancellationToken);

    public void Update(Recipe recipe) =>
        _dbContext.Recipes.Update(recipe);

    public void Remove(Recipe recipe) =>
        _dbContext.Recipes.Remove(recipe);
}
