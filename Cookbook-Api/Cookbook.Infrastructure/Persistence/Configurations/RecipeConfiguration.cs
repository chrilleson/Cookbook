using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Infrastructure.Persistence.Configurations.Comparers;
using Cookbook.Infrastructure.Persistence.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                id => id.Value,
                value => new RecipeId(value));

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        builder.Property(r => r.Instructions)
            .HasConversion<JsonArrayConverter<Instruction>>(new JsonArrayComparer<Instruction>())
            .HasField("_instructions")
            .HasColumnType("jsonb");

        builder.Property(r => r.Ingredients)
            .HasConversion<JsonArrayConverter<RecipeIngredient>>(new JsonArrayComparer<RecipeIngredient>())
            .HasField("_ingredients")
            .HasColumnType("jsonb");

        builder.Ignore(r => r.DomainEvents);
    }
}
