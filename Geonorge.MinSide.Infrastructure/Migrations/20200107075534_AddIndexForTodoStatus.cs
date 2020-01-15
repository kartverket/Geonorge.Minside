using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class AddIndexForTodoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Todo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todo_Status",
                table: "Todo",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Todo_Status",
                table: "Todo");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Todo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
