using Microsoft.EntityFrameworkCore.Migrations;

namespace Hiromi.Data.Migrations
{
    public partial class IsCompletedAndIsSuccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Reminders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "Reminders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "Reminders");
        }
    }
}
