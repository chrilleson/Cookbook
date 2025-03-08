using Cookbook.Domain.Recipe;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Recipe> Recipes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
