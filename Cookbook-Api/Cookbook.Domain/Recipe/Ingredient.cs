using Cookbook.Domain.Units;

namespace Cookbook.Domain.Recipe;

public sealed class Ingredient
{
    public string Name { get; init; }
    public double Amount { get; init; }
    public Fluid? Fluid { get; init; }
    public Weight? Weight { get; init; }
    public Piece? Piece { get; init; }

    public Ingredient(string name, double amount, Fluid? fluid, Weight? weight, Piece? piece)
    {
        Name = name;
        Amount = amount;
        Fluid = fluid;
        Weight = weight;
        Piece = piece;
    }
}