using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class ToDoMeetingIdAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todo_Meetings_MeetingId",
                table: "Todo");

            migrationBuilder.AlterColumn<int>(
                name: "MeetingId",
                table: "Todo",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "OrganizationNumber",
                table: "Todo",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_Meetings_MeetingId",
                table: "Todo",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todo_Meetings_MeetingId",
                table: "Todo");

            migrationBuilder.DropColumn(
                name: "OrganizationNumber",
                table: "Todo");

            migrationBuilder.AlterColumn<int>(
                name: "MeetingId",
                table: "Todo",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Todo_Meetings_MeetingId",
                table: "Todo",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
