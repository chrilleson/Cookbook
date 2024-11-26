using Cookbook.Domain.Recipe;
using Cookbook.Domain.Units;

namespace Cookbook.Application.Recipe.Models;

public record RecipeDto(string Name, IEnumerable<string> Instructions, IEnumerable<IngredientDto> Ingredients)
{
    public static Domain.Recipe.Recipe ToEntity(RecipeDto dto)
    {
        var instructions = dto.Instructions
            .Select((instruction, index) => (index, instruction))
            .ToDictionary(x => x.index, x => x.instruction);
        var ingredients = dto.Ingredients
            .Select(ingredient => new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit.Fluid, ingredient.Unit.Weight))
            .ToList();

        return new Domain.Recipe.Recipe(dto.Name, instructions, ingredients);
    }

    public static RecipeDto FromEntity(Domain.Recipe.Recipe recipe)
    {
        var instructions = recipe.Instructions
            .Select(instruction => instruction.Value)
            .ToList();

        var ingredients = recipe.Ingredients
            .Select(ingredient => new IngredientDto(ingredient.Name, ingredient.Amount, new Unit(Fluid: ingredient.Fluid, Weight: ingredient.Weight)))
            .ToList();

        return new RecipeDto(recipe.Name, instructions, ingredients);
    }
};

public record IngredientDto(string Name, double Amount, Unit Unit);

public record Unit(Fluid? Fluid = null, Weight? Weight = null)
{
    public static Unit FromFluid(Fluid fluid) => new(fluid, Weight: null);
    public static Unit FromWeight(Weight weight) => new(Fluid: null, weight);
}