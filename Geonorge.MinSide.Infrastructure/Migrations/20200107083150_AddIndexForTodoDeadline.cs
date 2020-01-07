using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class AddIndexForTodoDeadline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Todo_Deadline",
                table: "Todo",
                column: "Deadline");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Todo_Deadline",
                table: "Todo");
        }
    }
}
