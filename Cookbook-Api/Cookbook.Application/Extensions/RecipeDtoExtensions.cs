using Cookbook.Application.Recipe.Models;

namespace Cookbook.Application.Extensions;

internal static class RecipeDtoExtensions
{
    public static RecipeDto FromEntity(this Domain.Recipe.Recipe recipe)
    {
        var instructions = recipe.Instructions
            .Select(instruction => instruction.Value)
            .ToList();

        var ingredients = recipe.Ingredients
            .Select(ingredient => new IngredientDto(ingredient.Name, ingredient.Amount, new Unit(Fluid: ingredient.Fluid, Weight: ingredient.Weight, Piece: ingredient.Piece)))
            .ToList();

        return new RecipeDto(recipe.Id, recipe.Name, recipe.Description, instructions, ingredients);
    }
}