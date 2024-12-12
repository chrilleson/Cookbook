using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Units;

namespace Cookbook.Tests.Application.Recipe.Models;

internal static class TestRecipe
{
    internal static RecipeDto CreateRecipeDto(
        int id = 1,
        string name = "My favourite recipe",
        IEnumerable<string> instructions = null,
        IEnumerable<IngredientDto> ingredients = null) =>
        new(id, name, instructions, ingredients);

    internal static IngredientDto CreateIngredientDto(
        string name = "Beef",
        double amount = 500,
        Unit unit = null) =>
        new(name, amount, unit ?? Unit.FromWeight(Weight.G));
}