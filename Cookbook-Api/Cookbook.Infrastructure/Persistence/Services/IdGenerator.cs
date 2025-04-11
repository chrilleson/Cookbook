using Cookbook.Application.Recipe.Services;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Persistence.Services;

public class IdGenerator : IIdGenerator
{
    private readonly AppDbContext _dbContext;

    private const string RecipeIdSequenceName = "Recipe_Id_seq";

    public IdGenerator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GenerateNextId(CancellationToken cancellationToken = default) =>
        (await _dbContext.Database
            .SqlQueryRaw<int>($"""SELECT nextval('"{RecipeIdSequenceName}"')""")
            .ToListAsync(cancellationToken))
        .First();
}
