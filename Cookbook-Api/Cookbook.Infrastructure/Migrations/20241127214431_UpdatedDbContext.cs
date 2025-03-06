using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdatedDbContext : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_Recipe",
            table: "Recipe");

        migrationBuilder.RenameTable(
            name: "Recipe",
            newName: "Recipes");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Recipes",
            table: "Recipes",
            column: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_Recipes",
            table: "Recipes");

        migrationBuilder.RenameTable(
            name: "Recipes",
            newName: "Recipe");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Recipe",
            table: "Recipe",
            column: "Id");
    }
}
