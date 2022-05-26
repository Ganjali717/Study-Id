using System;
using Microsoft.EntityFrameworkCore.Migrations;
using StudyId.Data.SqlScripts;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class SID31addrelationalmodelforcourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course",
                table: "Applications");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationsCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationsCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationsCourses_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationsCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationsCourses_ApplicationId",
                table: "ApplicationsCourses",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationsCourses_CourseId",
                table: "ApplicationsCourses",
                column: "CourseId");

            var sql = SqlHelper.ReadScript("SID-31-insert-to-courses-table-the-list-of-courses.sql");
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationsCourses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Course",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
