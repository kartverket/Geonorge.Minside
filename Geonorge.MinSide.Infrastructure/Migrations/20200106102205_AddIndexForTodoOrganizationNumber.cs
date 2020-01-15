using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class AddIndexForTodoOrganizationNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganizationNumber",
                table: "Todo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todo_OrganizationNumber",
                table: "Todo",
                column: "OrganizationNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Todo_OrganizationNumber",
                table: "Todo");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationNumber",
                table: "Todo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
