﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(builder => builder.ConfigureDbContext(connectionString));

        return services;
    }
    private static void ConfigureDbContext(this DbContextOptionsBuilder builder, string connectionString) =>
         builder
             .UseNpgsql(connectionString)
             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
             .ConfigureWarnings(x => x.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
}