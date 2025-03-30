using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRowVersionTrigger : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Ensure pgcrypto extension is enabled
        migrationBuilder.Sql(@"
            CREATE EXTENSION IF NOT EXISTS pgcrypto;
        ");

        // Create function to generate new row version
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION update_row_version()
            RETURNS TRIGGER AS $$
            BEGIN
                NEW.""RowVersion"" = decode(encode(gen_random_bytes(8), 'hex'), 'hex');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // Create trigger that uses the function for updates
        migrationBuilder.Sql(@"
            CREATE TRIGGER update_recipes_row_version
            BEFORE UPDATE ON ""Recipes""
            FOR EACH ROW
            EXECUTE FUNCTION update_row_version();
        ");

        // Create trigger that uses the function for inserts
        migrationBuilder.Sql(@"
            CREATE TRIGGER insert_recipes_row_version
            BEFORE INSERT ON ""Recipes""
            FOR EACH ROW
            EXECUTE FUNCTION update_row_version();
        ");

        // Initialize row version for existing records
        migrationBuilder.Sql(@"
            UPDATE ""Recipes""
            SET ""RowVersion"" = decode(encode(gen_random_bytes(8), 'hex'), 'hex');
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS update_recipes_row_version ON \"Recipes\";");
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS insert_recipes_row_version ON \"Recipes\";");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_row_version();");
        migrationBuilder.Sql("DROP EXTENSION IF EXISTS pgcrypto;");
    }
}
