using Cookbook.Application.Repositories;
using Cookbook.Application.UnitOfWork;
using Cookbook.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cookbook.Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgres", configureDbContextOptions: options =>
        {
            options.ConfigureWarnings(x => x.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.UseNpgsql(opt => opt.ConfigureDataSource(x => x.EnableDynamicJson()));
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        return builder;
    }

    public static WebApplicationBuilder AddRedisOutputCache(this WebApplicationBuilder builder)
    {
        builder.AddRedisOutputCache(connectionName: "redis");

        return builder;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        return services;
    }

    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!(await dbContext.Database.GetPendingMigrationsAsync()).Any()) return;

        await dbContext.Database.MigrateAsync();
    }
}