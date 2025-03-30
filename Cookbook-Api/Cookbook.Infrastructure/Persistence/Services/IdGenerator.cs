using Cookbook.Application.Recipe.Services;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Persistence.Services;

public class IdGenerator : IIdGenerator
{
    private readonly AppDbContext _dbContext;
    private readonly string _sequenceName;

    public IdGenerator(AppDbContext dbContext, string sequenceName = "recipe_id_seq")
    {
        _dbContext = dbContext;
        _sequenceName = sequenceName;
    }

    public int GenerateNextId()
    {
        var result = _dbContext.Database
            .ExecuteSql($"SELECT nextval('{_sequenceName}')");

        return result;
    }
}
