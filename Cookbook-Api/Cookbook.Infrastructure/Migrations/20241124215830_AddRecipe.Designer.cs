﻿// <auto-generated />
using System.Collections.Generic;
using Cookbook.Domain.Recipe;
using Cookbook.Domain.Recipe.Entities;
using Cookbook.Domain.Recipe.ValueObjects;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cookbook.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241124215830_AddRecipe")]
    partial class AddRecipe
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Cookbook.Domain.Recipe.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<IEnumerable<Ingredient>>("Ingredients")
                        .HasColumnType("jsonb");

                    b.Property<Dictionary<int, string>>("Instructions")
                        .HasColumnType("jsonb");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Recipe");
                });
#pragma warning restore 612, 618
        }
    }
}
