using Microsoft.EntityFrameworkCore.Migrations;
using StudyId.Data.SqlScripts;

#nullable disable

namespace StudyId.Data.Migrations
{
    public partial class SID6addaccountsmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = SqlHelper.ReadScript("SID-6-create-base-superadmin-account.sql");
            migrationBuilder.Sql(sql); //pwd:ppKJGX^28z2t
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
