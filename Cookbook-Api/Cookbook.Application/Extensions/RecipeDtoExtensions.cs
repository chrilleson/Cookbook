using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe;

namespace Cookbook.Application.Extensions;

internal static class RecipeDtoExtensions
{
    public static Domain.Recipe.Recipe ToEntity(this RecipeDto dto)
    {
        var instructions = dto.Instructions!
            .Select((instruction, index) => (index, instruction))
            .ToDictionary(x => x.index, x => x.instruction);
        var ingredients = dto.Ingredients!
            .Select(ingredient => new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit.Fluid, ingredient.Unit.Weight, ingredient.Unit.Piece))
            .ToList();

        return new Domain.Recipe.Recipe
        {
            Id = dto.Id,
            Name = dto.Name!,
            Description = dto.Description!,
            Instructions = instructions,
            Ingredients = ingredients
        };
    }

    public static Domain.Recipe.Recipe ToEntity(this CreateRecipeDto dto)
    {
        var instructions = dto.Instructions
            .Select((instruction, index) => (index, instruction))
            .ToDictionary(x => x.index, x => x.instruction);
        var ingredients = dto.Ingredients
            .Select(ingredient => new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit.Fluid, ingredient.Unit.Weight, ingredient.Unit.Piece))
            .ToList();

        return new Domain.Recipe.Recipe { Name = dto.Name, Description = dto.Description, Instructions = instructions, Ingredients = ingredients };
    }
}