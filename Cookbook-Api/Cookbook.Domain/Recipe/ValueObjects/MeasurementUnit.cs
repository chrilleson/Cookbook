using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Domain.Recipe.ValueObjects;

public abstract record MeasurementUnit
{
    public string Symbol { get; }

    protected MeasurementUnit(string symbol)
    {
        Symbol = symbol;
    }

    public override string ToString() => Symbol;

    public static MeasurementUnit Fluid(Fluid unit) => new FluidMeasurement(unit);
    public static MeasurementUnit Weight(Weight unit) => new WeightMeasurement(unit);
    public static MeasurementUnit Piece() => new PieceMeasurement();
}

public record FluidMeasurement : MeasurementUnit
{
    public Fluid Unit { get; }

    public FluidMeasurement(Fluid unit) : base(unit.ToString())
    {
        Unit = unit;
    }
}

public record WeightMeasurement : MeasurementUnit
{
    public Weight Unit { get; }

    public WeightMeasurement(Weight unit) : base(unit.ToString())
    {
        Unit = unit;
    }
}

public record PieceMeasurement : MeasurementUnit
{
    public PieceMeasurement() : base("pc") { }
}
