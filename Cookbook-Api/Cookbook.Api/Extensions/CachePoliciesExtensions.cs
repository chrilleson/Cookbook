using Cookbook.Api.Constants;

namespace Cookbook.Api.Extensions;

public static class CachePoliciesExtensions
{
    public static void AddCachePolicies(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddPolicy(RecipeConstants.CachePolicyNameListRecipes, builder =>
            {
                builder.Tag(RecipeConstants.CacheTagListRecipes);
                builder.SetVaryByHost(true);
            });

            options.AddPolicy(RecipeConstants.CachePolicyNameRecipeById, builder =>
            {
                builder.Tag(RecipeConstants.CacheTagRecipeById);
                builder.SetVaryByHost(true);
                builder.SetVaryByRouteValue("id");
            });
        });
    }
}
