using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class AddImageTagsFilesForArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageTags",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageTags",
                table: "Articles");
        }
    }
}
