using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Tests.Application.Recipe.Models;

internal static class TestRecipe
{
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
        IEnumerable<Instruction>? instructions = null,
        IEnumerable<RecipeIngredient>? ingredients = null)
    {
        var recipe = new Domain.Recipe.Entities.Recipe(
            id: new RecipeId(id),
            name,
            description
        );

        foreach (var instruction in instructions)
        {
            recipe.AddInstruction(instruction.Text);
        }
        foreach (var ingredient in ingredients)
        {
            recipe.AddIngredient(ingredient.Name, ingredient.Quantity.Amount, ingredient.Quantity.Unit);
        }

        return recipe;
    }

    internal static Instruction CreateInstruction(int stepNumber = 0, string text = "First Instruction") => new(stepNumber, text);

    internal static RecipeIngredient CreateRecipeIngredient(string name = "Beef", double amount = 500, MeasurementUnit? unit = null)
    {
        var quantity = new Quantity(amount, unit ?? MeasurementUnit.Weight(Weight.G));
        return new RecipeIngredient(name, quantity);
    }
}
