using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class AddValidaDataTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityTockenExpired",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "SecurityTocken",
                table: "Accounts",
                newName: "SecurityToken");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SecurityTokenExpired",
                table: "Accounts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityTokenExpired",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "SecurityToken",
                table: "Accounts",
                newName: "SecurityTocken");

            migrationBuilder.AddColumn<string>(
                name: "SecurityTockenExpired",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
