namespace Cookbook.Application.Recipe;

public static class RecipeRoutes
{
    public const string GetRecipe = "/api/recipes/{0}";
    public const string GetAllRecipes = "/api/recipes";
    public const string CreateRecipe = "/api/recipes";
    public const string UpdateRecipe = "/api/recipes/{0}";
    public const string DeleteRecipe = "/api/recipes/{0}";

    public static string FormatRoute(string route, params object[] args) =>
        string.Format(route, args);
}