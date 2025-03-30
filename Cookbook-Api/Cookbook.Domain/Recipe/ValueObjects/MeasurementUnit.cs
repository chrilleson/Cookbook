using System.Text.Json.Serialization;
using Cookbook.Domain.Recipe.Converters;
using Cookbook.Domain.Shared.Enums;

namespace Cookbook.Domain.Recipe.ValueObjects;

[JsonConverter(typeof(MeasurementUnitJsonConverter))]
public record MeasurementUnit
{
    public string Symbol { get; }
    public string UnitType { get; }

    [JsonConstructor]
    protected MeasurementUnit(string symbol, string unitType)
    {
        Symbol = symbol;
        UnitType = unitType;
    }

    public override string ToString() => Symbol;

    public static MeasurementUnit Fluid(Fluid unit) => new FluidMeasurement(unit);
    public static MeasurementUnit Weight(Weight unit) => new WeightMeasurement(unit);
    public static MeasurementUnit Piece() => new PieceMeasurement();

    public static MeasurementUnit Create(string symbol, string unitType)
    {
        return unitType switch
        {
            "Fluid" => new FluidMeasurement(ParseFluidEnum(symbol)),
            "Weight" => new WeightMeasurement(ParseWeightEnum(symbol)),
            "Piece" => new PieceMeasurement(),
            _ => throw new ArgumentException($"Unknown unit type: {unitType}")
        };
    }

    private static Fluid ParseFluidEnum(string symbol) =>
        Enum.TryParse<Fluid>(symbol, true, out var result) ? result : throw new ArgumentException($"Invalid fluid symbol: {symbol}");

    private static Weight ParseWeightEnum(string symbol) =>
        Enum.TryParse<Weight>(symbol, true, out var result) ? result : throw new ArgumentException($"Invalid weight symbol: {symbol}");

}

public sealed record FluidMeasurement : MeasurementUnit
{
    public Fluid Unit { get; }

    public FluidMeasurement(Fluid unit) : base(unit.ToString(), "Fluid")
    {
        Unit = unit;
    }
}

public sealed record WeightMeasurement : MeasurementUnit
{
    public Weight Unit { get; }

    public WeightMeasurement(Weight unit) : base(unit.ToString(), "Weight")
    {
        Unit = unit;
    }
}

public sealed record PieceMeasurement : MeasurementUnit
{
    public PieceMeasurement() : base("Piece", "Piece") { }
}
