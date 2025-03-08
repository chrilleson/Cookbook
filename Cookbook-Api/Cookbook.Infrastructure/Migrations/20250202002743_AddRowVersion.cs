using System.Collections.Generic;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRowVersion : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Dictionary<int, string>>(
            name: "Instructions",
            table: "Recipes",
            type: "jsonb",
            nullable: false,
            oldClrType: typeof(Dictionary<int, string>),
            oldType: "jsonb",
            oldNullable: true);

        migrationBuilder.AlterColumn<IEnumerable<Ingredient>>(
            name: "Ingredients",
            table: "Recipes",
            type: "jsonb",
            nullable: false,
            oldClrType: typeof(IEnumerable<Ingredient>),
            oldType: "jsonb",
            oldNullable: true);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "Recipes",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            defaultValue: new byte[0]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "Recipes");

        migrationBuilder.AlterColumn<Dictionary<int, string>>(
            name: "Instructions",
            table: "Recipes",
            type: "jsonb",
            nullable: true,
            oldClrType: typeof(Dictionary<int, string>),
            oldType: "jsonb");

        migrationBuilder.AlterColumn<IEnumerable<Ingredient>>(
            name: "Ingredients",
            table: "Recipes",
            type: "jsonb",
            nullable: true,
            oldClrType: typeof(IEnumerable<Ingredient>),
            oldType: "jsonb");
    }
}
