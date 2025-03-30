using Cookbook.Application.Recipe.Models;
using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Application.Extensions;

public static class RecipeExtensions
{
    public static RecipeDto MapToDto(this Domain.Recipe.Entities.Recipe recipe)
    {
        return new RecipeDto(
            recipe.Id.Value,
            recipe.Name,
            recipe.Description,
            recipe.Instructions.OrderBy(i => i.StepNumber).Select(i => i.Text).ToList(),
            recipe.Ingredients.Select(i => new IngredientDto(
                i.Name,
                i.Quantity.Amount,
                new Unit(
                    Fluid: i.Quantity.Unit.UnitType == "Fluid" ? Enum.Parse<Fluid>(i.Quantity.Unit.Symbol) : null,
                    Weight: i.Quantity.Unit.UnitType == "Weight" ? Enum.Parse<Weight>(i.Quantity.Unit.Symbol) : null,
                    Piece: i.Quantity.Unit.UnitType == "Piece" ? Piece.Piece : null
                )
            )).ToList(),
            recipe.RowVersion ?? []
        );
    }
}
