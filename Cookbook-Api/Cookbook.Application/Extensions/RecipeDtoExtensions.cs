using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe;

namespace Cookbook.Application.Extensions;

internal static class RecipeDtoExtensions
{
    public static Domain.Recipe.Recipe ToEntity(this RecipeDto dto)
    {
        var instructions = dto.Instructions
            .Select((instruction, index) => (index, instruction))
            .ToDictionary(x => x.index, x => x.instruction);
        var ingredients = dto.Ingredients
            .Select(ingredient => new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit.Fluid, ingredient.Unit.Weight))
            .ToList();

        return new Domain.Recipe.Recipe(dto.Name, instructions, ingredients);
    }

    public static RecipeDto FromEntity(this Domain.Recipe.Recipe recipe)
    {
        var instructions = recipe.Instructions
            .Select(instruction => instruction.Value)
            .ToList();

        var ingredients = recipe.Ingredients
            .Select(ingredient => new IngredientDto(ingredient.Name, ingredient.Amount, new Unit(Fluid: ingredient.Fluid, Weight: ingredient.Weight)))
            .ToList();

        return new RecipeDto(recipe.Name, instructions, ingredients);
    }
}