﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Recipes.Database;

#nullable disable

namespace Recipes.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Recipes.Model.CookingTimes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CookingTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CookingTimes");
                });

            modelBuilder.Entity("Recipes.Model.Ingredients", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Ingredient")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Recipes.Model.RecipeIngredients", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<int>("IngredientId")
                        .HasColumnType("int");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("RecipeId", "IngredientId");

                    b.HasIndex("IngredientId");

                    b.HasIndex("UnitId");

                    b.ToTable("RecipeIngredients");
                });

            modelBuilder.Entity("Recipes.Model.RecipeRecipeTags", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<int>("RecipeTagId")
                        .HasColumnType("int");

                    b.HasKey("RecipeId", "RecipeTagId");

                    b.HasIndex("RecipeTagId");

                    b.ToTable("RecipeRecipeTags");
                });

            modelBuilder.Entity("Recipes.Model.RecipeTags", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RecipeTags");
                });

            modelBuilder.Entity("Recipes.Model.Recipes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CookingInstructions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CookingTimeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("bit");

                    b.Property<string>("Recipe")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CookingTimeId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("Recipes.Model.Units", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("Recipes.Model.RecipeIngredients", b =>
                {
                    b.HasOne("Recipes.Model.Ingredients", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Recipes.Model.Recipes", "Recipes")
                        .WithMany("RecipeIngredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Recipes.Model.Units", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Recipes");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Recipes.Model.RecipeRecipeTags", b =>
                {
                    b.HasOne("Recipes.Model.Recipes", "Recipe")
                        .WithMany("RecipeRecipeTags")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Recipes.Model.RecipeTags", "RecipeTag")
                        .WithMany("RecipeRecipeTags")
                        .HasForeignKey("RecipeTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");

                    b.Navigation("RecipeTag");
                });

            modelBuilder.Entity("Recipes.Model.Recipes", b =>
                {
                    b.HasOne("Recipes.Model.CookingTimes", "CookingTime")
                        .WithMany()
                        .HasForeignKey("CookingTimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CookingTime");
                });

            modelBuilder.Entity("Recipes.Model.RecipeTags", b =>
                {
                    b.Navigation("RecipeRecipeTags");
                });

            modelBuilder.Entity("Recipes.Model.Recipes", b =>
                {
                    b.Navigation("RecipeIngredients");

                    b.Navigation("RecipeRecipeTags");
                });
#pragma warning restore 612, 618
        }
    }
}
