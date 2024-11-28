using Cookbook.Domain.Recipe;
using Cookbook.Domain.Units;

namespace Cookbook.Application.Recipe.Models;

public record RecipeDto(string Name, IEnumerable<string> Instructions, IEnumerable<IngredientDto> Ingredients);

public record IngredientDto(string Name, double Amount, Unit Unit);

public record Unit(Fluid? Fluid = null, Weight? Weight = null)
{
    public static Unit FromFluid(Fluid fluid) => new(fluid, Weight: null);
    public static Unit FromWeight(Weight weight) => new(Fluid: null, weight);
}