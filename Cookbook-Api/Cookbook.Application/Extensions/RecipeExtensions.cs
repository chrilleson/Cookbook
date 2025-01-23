using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe;

namespace Cookbook.Application.Extensions;

public static class RecipeExtensions
{
    public static Domain.Recipe.Recipe ToEntity(this RecipeDto dto)
    {
        var instructions = dto.Instructions!
            .Select((instruction, index) => (index, instruction))
            .ToDictionary(x => x.index, x => x.instruction);
        var ingredients = dto.Ingredients!
            .Select(ingredient => new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit.Fluid, ingredient.Unit.Weight, ingredient.Unit.Piece))
            .ToList();

        return new Domain.Recipe.Recipe(dto.Id, dto.Name!, instructions, ingredients);
    }

    public static Domain.Recipe.Recipe Update(this Domain.Recipe.Recipe recipe, RecipeDto dto)
    {
        recipe.Name = dto.Name ?? recipe.Name;
        recipe.Instructions = dto.Instructions?
            .Select((instruction, index) => (index, instruction))
            .ToDictionary(x => x.index, x => x.instruction) ?? recipe.Instructions;
        recipe.Ingredients = dto.Ingredients?
            .Select(ingredient => new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit.Fluid, ingredient.Unit.Weight, ingredient.Unit.Piece))
            .ToList() ?? recipe.Ingredients;

        return recipe;
    }
}