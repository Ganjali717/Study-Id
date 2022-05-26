using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class AddPermamentFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "Articles");
        }
    }
}
