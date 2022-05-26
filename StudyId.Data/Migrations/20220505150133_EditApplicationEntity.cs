using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class EditApplicationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Courses_CourseId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_CourseId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Applications");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationsCourses");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CourseId",
                table: "Applications",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Courses_CourseId",
                table: "Applications",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
