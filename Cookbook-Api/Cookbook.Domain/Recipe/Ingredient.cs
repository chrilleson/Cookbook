namespace Cookbook.Domain.Recipe;

public class Ingredient
{
    public string Name { get; set; }
    public double Amount { get; set; }
    public Units.Fluid? Fluid { get; set; }
    public Units.Weight? Weight { get; set; }

    public Ingredient(string name, double amount, Units.Fluid? fluid, Units.Weight? weight)
    {
        Name = name;
        Amount = amount;
        Fluid = fluid;
        Weight = weight;
    }
}