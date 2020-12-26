using Microsoft.EntityFrameworkCore.Migrations;

namespace Hiromi.Data.Migrations
{
    public partial class RefactorTagsPerChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowQuotes",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "AllowTags",
                table: "Guilds");

            migrationBuilder.AddColumn<bool>(
                name: "AllowTags",
                table: "Channels",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowTags",
                table: "Channels");

            migrationBuilder.AddColumn<bool>(
                name: "AllowQuotes",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowTags",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
