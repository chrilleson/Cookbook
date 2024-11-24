using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Repositories;

public static class RepositoriesExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }
}