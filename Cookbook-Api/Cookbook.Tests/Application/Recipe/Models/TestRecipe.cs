using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Tests.Application.Recipe.Models;

internal static class TestRecipe
{
    internal static RecipeDto CreateRecipeDto(
        int id = 1,
        string name = "My favourite recipe",
        string description = "This is my favourite recipe",
        IEnumerable<string>? instructions = null,
        IEnumerable<IngredientDto>? ingredients = null,
        byte[]? rowVersion = null) =>
        new(id, name, description, instructions!, ingredients!, rowVersion!);

    internal static IngredientDto CreateIngredientDto(
        string name = "Beef",
        double amount = 500,
        Unit? unit = null) =>
        new(name, amount, unit ?? Unit.FromWeight(Weight.G));

    internal static CreateRecipeDto CreateCreateRecipeDto(
        string name = "My favourite recipe",
        string description = "This is my favourite recipe",
        IEnumerable<string>? instructions = null,
        IEnumerable<IngredientDto>? ingredients = null) =>
        new(name, description, instructions!, ingredients!);

    internal static UpdateRecipeDto CreateUpdateRecipeDto(
        string name = "My favourite recipe",
        string description = "This is my favourite recipe",
        IEnumerable<string>? instructions = null,
        IEnumerable<IngredientDto>? ingredients = null,
        byte[]? rowVersion = null) =>
        new(name, description, instructions, ingredients, rowVersion!);

    internal static Domain.Recipe.Entities.Recipe CreateRecipe(
        int id = 1,
        string name = "My favourite recipe",
        string description = "This is my favourite recipe",
        Dictionary<int, string>? instructions = null,
        IEnumerable<Ingredient>? ingredients = null,
        byte[]? rowVersion = null) =>
        new() { Id = id, Name = name, Description = description, Instructions = instructions!, Ingredients = ingredients!, RowVersion = rowVersion! };

    internal static Ingredient CreateIngredient(
        string name = "Beef",
        double amount = 500,
        Fluid? fluid = null,
        Weight? weight = null,
        Piece? piece = null) =>
        new(name, amount, fluid, weight, piece);
}
