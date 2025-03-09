using Cookbook.Domain.Recipe.ValueObjects;

namespace Cookbook.Domain.Recipe.Entities;

public class RecipeIngredient
{
    public string Name { get; private set; }
    public Quantity Quantity { get; private set; }

    // For EF Core
    private RecipeIngredient() { }

    public RecipeIngredient(string name, Quantity quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ingredient name cannot be empty", nameof(name));

        Name = name;
        Quantity = quantity;
    }

    public void UpdateQuantity(Quantity quantity)
    {
        Quantity = quantity;
    }
}
