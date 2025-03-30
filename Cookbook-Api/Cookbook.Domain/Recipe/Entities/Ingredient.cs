using Ardalis.GuardClauses;
using Cookbook.Domain.Recipe.ValueObjects;

namespace Cookbook.Domain.Recipe.Entities;

public sealed class Ingredient
{
    public string Name { get; private set; }
    public Quantity Quantity { get; private set; }

    public Ingredient(string name, Quantity quantity)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, exceptionCreator: () => new ArgumentException("Ingredient name cannot be empty", nameof(name)));
        Quantity = quantity;
    }

    public void UpdateQuantity(Quantity quantity) =>
        Quantity = quantity;
}
