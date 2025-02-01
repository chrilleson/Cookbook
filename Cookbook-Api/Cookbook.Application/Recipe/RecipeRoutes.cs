namespace Cookbook.Application.Recipe;

public static class RecipeRoutes
{
    public const string GetRecipe = "/api/recipe/{id}";
    public const string GetAllRecipes = "/api/recipe";
    public const string CreateRecipe = "/api/recipe";
    public const string UpdateRecipe = "/api/recipe/{id}";
    public const string DeleteRecipe = "/api/recipe/{id}";

    public static string FormatRoute(string route, params object[] args) =>
        string.Format(route, args);
}