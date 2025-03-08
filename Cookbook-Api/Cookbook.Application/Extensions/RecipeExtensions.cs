using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;

namespace Cookbook.Application.Extensions;

public static class RecipeExtensions
{
    public static RecipeDto ToDto(this Domain.Recipe.Entities.Recipe recipe)
    {
        var instructions = recipe.Instructions
            .Select(instruction => instruction.Value)
            .ToList();

        var ingredients = recipe.Ingredients
            .Select(ingredient => new IngredientDto(ingredient.Name, ingredient.Amount, new Unit(Fluid: ingredient.Fluid, Weight: ingredient.Weight, Piece: ingredient.Piece)))
            .ToList();

        return new RecipeDto(recipe.Id, recipe.Name, recipe.Description, instructions, ingredients, recipe.RowVersion ?? []);
    }

    public static Domain.Recipe.Entities.Recipe Update(this Domain.Recipe.Entities.Recipe recipe, UpdateRecipeDto dto)
    {
        recipe.Name = dto.Name ?? recipe.Name;
        recipe.Description = dto.Description ?? recipe.Description;

        if (dto.Instructions is not null)
        {
            recipe.Instructions = dto.Instructions
                .Select((instruction, index) => (index, instruction))
                .ToDictionary(x => x.index, x => x.instruction);
        }

        if (dto.Ingredients is not null)
        {
            recipe.Ingredients = dto.Ingredients
                .Select(ingredient => new Ingredient(
                    ingredient.Name,
                    ingredient.Amount,
                    ingredient.Unit.Fluid,
                    ingredient.Unit.Weight,
                    ingredient.Unit.Piece))
                .ToList();
        }

        return recipe;
    }
}
