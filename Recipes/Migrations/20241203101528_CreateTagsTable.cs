using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Migrations
{
    /// <inheritdoc />
    public partial class CreateTagsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodCategories");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropColumn(
                name: "FoodCategoryId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "MealId",
                table: "Recipes");

            migrationBuilder.CreateTable(
                name: "RecipeTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeRecipeTags",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    RecipeTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeRecipeTags", x => new { x.RecipeId, x.RecipeTagId });
                    table.ForeignKey(
                        name: "FK_RecipeRecipeTags_RecipeTags_RecipeTagId",
                        column: x => x.RecipeTagId,
                        principalTable: "RecipeTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeRecipeTags_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeRecipeTags_RecipeTagId",
                table: "RecipeRecipeTags",
                column: "RecipeTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeRecipeTags");

            migrationBuilder.DropTable(
                name: "RecipeTags");

            migrationBuilder.AddColumn<int>(
                name: "FoodCategoryId",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MealId",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FoodCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodCategory = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Meal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                });
        }
    }
}
