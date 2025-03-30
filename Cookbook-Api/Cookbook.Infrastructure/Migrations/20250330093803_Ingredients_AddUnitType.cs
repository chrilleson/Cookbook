using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Ingredients_AddUnitType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                             UPDATE "Recipes"
                             SET "Ingredients" = (
                                 SELECT jsonb_agg(
                                     jsonb_build_object(
                                         'name', item->>'name',
                                         'quantity', jsonb_build_object(
                                             'amount', (item->'quantity'->>'amount')::numeric,
                                             'unit', jsonb_build_object(
                                                 'symbol', item->'quantity'->'unit'->>'symbol',
                                                 'unitType', CASE
                                                     WHEN item->'quantity'->'unit'->>'symbol' IN ('L', 'Dl', 'Cl', 'Ml') THEN 'Fluid'
                                                     WHEN item->'quantity'->'unit'->>'symbol' IN ('Kg', 'G', 'Mg') THEN 'Weight'
                                                     WHEN item->'quantity'->'unit'->>'symbol' = 'Piece' THEN 'Piece'
                                                 END
                                             )
                                         )
                                     )
                                 )
                                 FROM jsonb_array_elements("Ingredients") AS item
                                 WHERE item->>'name' IS NOT NULL
                             )
                             WHERE "Ingredients" IS NOT NULL;
                             """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                             UPDATE "Recipes"
                             SET "Ingredients" = (
                                 SELECT jsonb_agg(
                                     jsonb_build_object(
                                         'name', item->>'name',
                                         'quantity', jsonb_build_object(
                                             'amount', (item->'quantity'->>'amount')::numeric,
                                             'unit', jsonb_build_object(
                                                 'symbol', item->'quantity'->'unit'->>'symbol'
                                                 -- unitType field is intentionally omitted here
                                             )
                                         )
                                     )
                                 )
                                 FROM jsonb_array_elements("Ingredients") AS item
                                 WHERE item->>'name' IS NOT NULL
                             )
                             WHERE "Ingredients" IS NOT NULL;
                             """);
    }
}
