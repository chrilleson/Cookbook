using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateInstructionObject : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                             UPDATE "Recipes"
                             SET "Instructions" = (
                                 SELECT jsonb_agg(
                                     jsonb_build_object(
                                         'stepNumber', (key::int),
                                         'text', value #>> '{}'
                                     )
                                     ORDER BY key::int
                                 )
                                 FROM jsonb_each("Instructions") AS items(key, value)
                             );
                             """);

        migrationBuilder.Sql("""
                             UPDATE "Recipes"
                             SET "Ingredients" = (
                                 SELECT jsonb_agg(
                                     jsonb_build_object(
                                         'name', item->>'Name',
                                         'quantity', jsonb_build_object(
                                             'amount', (item->>'Amount')::numeric,
                                             'unit', jsonb_build_object(
                                                 'symbol', CASE
                                                     WHEN (item->>'Piece') IS NOT NULL AND (item->>'Piece')::int = 0 THEN 'Piece'
                                                     WHEN (item->>'Fluid') IS NOT NULL AND (item->>'Fluid')::int = 0 THEN 'L'
                                                     WHEN (item->>'Fluid') IS NOT NULL AND (item->>'Fluid')::int = 1 THEN 'Dl'
                                                     WHEN (item->>'Fluid') IS NOT NULL AND (item->>'Fluid')::int = 2 THEN 'Cl'
                                                     WHEN (item->>'Fluid') IS NOT NULL AND (item->>'Fluid')::int = 3 THEN 'Ml'
                                                     WHEN (item->>'Weight') IS NOT NULL AND (item->>'Weight')::int = 0 THEN 'Kg'
                                                     WHEN (item->>'Weight') IS NOT NULL AND (item->>'Weight')::int = 1 THEN 'G'
                                                     WHEN (item->>'Weight') IS NOT NULL AND (item->>'Weight')::int = 2 THEN 'Mg'
                                                     ELSE NULL
                                                 END
                                             )
                                         )
                                     )
                                     ORDER BY item->>'Name'
                                 )
                                 FROM jsonb_array_elements("Ingredients") AS item
                                 WHERE item->>'Name' IS NOT NULL -- Ensure we have a name
                             );
                             """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
                             UPDATE "Recipes"
                             SET "Instructions" = (
                                 SELECT jsonb_object_agg(
                                     s.step_number::text, s.text
                                 )
                                 FROM (
                                     SELECT
                                         (elem->>'stepNumber')::int as step_number,
                                         elem->>'text' as text
                                     FROM jsonb_array_elements("Instructions") as elem
                                     ORDER BY (elem->>'stepNumber')::int
                                 ) s
                             );
                             """);

        migrationBuilder.Sql("""
                             UPDATE "Recipes"
                             SET "Ingredients" = (
                                 SELECT jsonb_agg(
                                     jsonb_build_object(
                                         'Name', item->>'name',
                                         'Fluid', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'L' THEN 0 ELSE NULL END,
                                         'Fluid', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'Dl' THEN 1 ELSE NULL END,
                                         'Fluid', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'Cl' THEN 2 ELSE NULL END,
                                         'Fluid', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'Ml' THEN 3 ELSE NULL END,
                                         'Piece', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'Piece' THEN 0 ELSE NULL END,
                                         'Amount', (item->'quantity'->>'amount')::numeric,
                                         'Weight', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'Kg' THEN 0 ELSE NULL END
                                         'Weight', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'G' THEN 1 ELSE NULL END
                                         'Weight', CASE WHEN item->'quantity'->'unit'->>'symbol' = 'KM' THEN 2 ELSE NULL END
                                     )
                                     ORDER BY item->>'name'
                                 )
                                 FROM jsonb_array_elements("Ingredients") AS item
                             );
                             """);
    }
}
