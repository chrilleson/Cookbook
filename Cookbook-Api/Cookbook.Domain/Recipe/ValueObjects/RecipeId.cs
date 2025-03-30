using Ardalis.GuardClauses;

namespace Cookbook.Domain.Recipe.ValueObjects;

public sealed record RecipeId
{
    public int Value { get; }

    public RecipeId(int value)
    {
        Value = Guard.Against.NegativeOrZero(value, exceptionCreator: () => new ArgumentException("Recipe ID must be positive", nameof(value)));
    }

    public static implicit operator int(RecipeId id) => id.Value;

    public override string ToString() => Value.ToString();
}
