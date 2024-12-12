using Cookbook.Domain.Units;

namespace Cookbook.Application.Recipe.Models;

public record RecipeDto(int Id, string Name, IEnumerable<string> Instructions, IEnumerable<IngredientDto> Ingredients);

public record IngredientDto(string Name, double Amount, Unit Unit);

public record Unit(Fluid? Fluid = null, Weight? Weight = null, Piece? Piece = null)
{
    public static Unit FromFluid(Fluid fluid) => new(fluid, Weight: null);
    public static Unit FromWeight(Weight weight) => new(Fluid: null, weight);
    public static Unit FromPiece(Piece piece) => new(Fluid: null, Weight: null, piece);
}