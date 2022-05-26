using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class AddRouteKeyToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Route",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RouteKey",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RouteKey1",
                table: "Categories",
                column: "RouteKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RouteKey1",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Route",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "RouteKey",
                table: "Categories");
        }
    }
}
