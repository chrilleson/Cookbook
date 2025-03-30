using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Cookbook.Domain.Recipe.ValueObjects;

namespace Cookbook.Domain.Recipe.Entities;

public class RecipeIngredient
{
    public string Name { get; private set; }
    public Quantity Quantity { get; private set; }

    private RecipeIngredient() { }

    public RecipeIngredient(string name, Quantity quantity)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, exceptionCreator: () => new ArgumentException("Ingredient name cannot be empty", nameof(name)));
        Quantity = quantity;
    }

    public void UpdateQuantity(Quantity quantity) =>
        Quantity = quantity;
}
