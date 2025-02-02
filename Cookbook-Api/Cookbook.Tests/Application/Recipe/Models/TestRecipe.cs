using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Units;

namespace Cookbook.Tests.Application.Recipe.Models;

internal static class TestRecipe
{
    internal static RecipeDto CreateRecipeDto(
        int id = 1,
        string name = "My favourite recipe",
        string description = "This is my favourite recipe",
        IEnumerable<string>? instructions = null,
        IEnumerable<IngredientDto>? ingredients = null) =>
        new(id, name, description, instructions, ingredients);

    internal static IngredientDto CreateIngredientDto(
        string name = "Beef",
        double amount = 500,
        Unit? unit = null) =>
        new(name, amount, unit ?? Unit.FromWeight(Weight.G));

    internal static Domain.Recipe.Recipe CreateRecipe(
        int id = 1,
        string name = "My favourite recipe",
        string description = "This is my favourite recipe",
        Dictionary<int, string>? instructions = null,
        IEnumerable<Ingredient>? ingredients = null) =>
        new(id, name, description, instructions, ingredients);

    internal static Ingredient CreateIngredient(
        string name = "Beef",
        double amount = 500,
        Fluid? fluid = null,
        Weight? weight = null,
        Piece? piece = null) =>
        new(name, amount, fluid, weight, piece);
}