using Ardalis.GuardClauses;

namespace Cookbook.Domain.Recipe.ValueObjects;

public sealed record Quantity
{
    public double Amount { get; }
    public MeasurementUnit Unit { get; }

    private Quantity() { }

    public Quantity(double amount, MeasurementUnit unit)
    {
        Amount = Guard.Against.NegativeOrZero(amount, exceptionCreator: () => new ArgumentException("Amount must be positive", nameof(amount)));
        Unit = unit;
    }

    public override string ToString() => $"{Amount} {Unit}";
}
