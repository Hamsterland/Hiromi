using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hiromi.Data.Migrations
{
    public partial class AddGuildIdToChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Commands",
                table: "Channels",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GuildId",
                table: "Channels",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Channels");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Commands",
                table: "Channels",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>));
        }
    }
}
