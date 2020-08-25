using Microsoft.EntityFrameworkCore.Migrations;

namespace Hiromi.Data.Migrations
{
    public partial class RemoveReminderIsSuccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "Reminders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "Reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
