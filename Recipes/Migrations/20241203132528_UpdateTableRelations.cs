using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CookingTimeId",
                table: "Recipes",
                column: "CookingTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_CookingTimes_CookingTimeId",
                table: "Recipes",
                column: "CookingTimeId",
                principalTable: "CookingTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_CookingTimes_CookingTimeId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CookingTimeId",
                table: "Recipes");
        }
    }
}
