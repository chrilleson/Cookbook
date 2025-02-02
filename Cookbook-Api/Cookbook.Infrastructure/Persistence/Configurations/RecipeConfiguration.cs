using Cookbook.Domain.Recipe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.Ingredients)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(r => r.Instructions)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(r => r.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
    }
}