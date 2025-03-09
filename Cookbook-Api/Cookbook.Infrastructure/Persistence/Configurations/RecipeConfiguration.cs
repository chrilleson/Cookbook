using System.Text.Json;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;
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
            .HasConversion(
                instructions => JsonSerializer.Serialize(instructions, new JsonSerializerOptions()),
                json => JsonSerializer.Deserialize<List<Instruction>>(json, new JsonSerializerOptions()) ?? new List<Instruction>())
            .HasColumnType("jsonb");

        builder.Property(r => r.Ingredients)
            .HasConversion(
                ingredients => JsonSerializer.Serialize(ingredients, new JsonSerializerOptions()),
                json => JsonSerializer.Deserialize<List<RecipeIngredient>>(json, new JsonSerializerOptions()) ?? new List<RecipeIngredient>())
            .HasColumnType("jsonb");

        builder.Ignore(r => r.DomainEvents);
    }
}
