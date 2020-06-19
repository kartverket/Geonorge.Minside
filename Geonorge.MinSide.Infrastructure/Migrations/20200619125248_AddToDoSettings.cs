using Microsoft.EntityFrameworkCore.Migrations;

namespace Geonorge.MinSide.Infrastructure.Migrations
{
    public partial class AddToDoSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TodoNotification",
                table: "UserSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TodoReminder",
                table: "UserSettings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TodoReminderTime",
                table: "UserSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TodoNotification",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "TodoReminder",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "TodoReminderTime",
                table: "UserSettings");
        }
    }
}
