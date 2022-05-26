using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class ChangeStartDateToDueDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Tasks",
                newName: "DueDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Tasks",
                newName: "StartDate");
        }
    }
}
