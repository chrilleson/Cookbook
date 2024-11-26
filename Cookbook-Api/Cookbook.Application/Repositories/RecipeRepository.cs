using Cookbook.Infrastructure.Persistence;
using Cookbook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Application.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _dbContext;

    public RecipeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Domain.Recipe.Recipe recipe) =>
        await _dbContext.Recipes.AddAsync(recipe);

    public async Task<Domain.Recipe.Recipe> GetById(int id) =>
        await _dbContext.Recipes.FindAsync(id);

    public async Task<IEnumerable<Domain.Recipe.Recipe>> GetAll() =>
        await _dbContext.Recipes.ToListAsync();
}