using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hiromi.Data.Migrations
{
    public partial class RemoveChannelCommandsRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Commands",
                table: "Channels",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Commands",
                table: "Channels",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldNullable: true);
        }
    }
}
