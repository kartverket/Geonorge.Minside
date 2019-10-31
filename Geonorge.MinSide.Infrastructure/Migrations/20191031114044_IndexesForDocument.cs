using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class IndexesForDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Date",
                table: "Documents",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FileName",
                table: "Documents",
                column: "FileName",
                unique: true,
                filter: "[FileName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Name",
                table: "Documents",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documents_Date",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_FileName",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_Name",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
