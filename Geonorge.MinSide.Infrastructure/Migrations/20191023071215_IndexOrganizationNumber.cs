using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class IndexOrganizationNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganizationNumber",
                table: "Meetings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationNumber",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_OrganizationNumber",
                table: "Meetings",
                column: "OrganizationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OrganizationNumber",
                table: "Documents",
                column: "OrganizationNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meetings_OrganizationNumber",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Documents_OrganizationNumber",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationNumber",
                table: "Meetings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationNumber",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
