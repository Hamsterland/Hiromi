using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Hiromi.Data.Migrations
{
    public partial class CreateLogChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogChannels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<long>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogChannels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogChannels_ChannelId",
                table: "LogChannels",
                column: "ChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogChannels_GuildId",
                table: "LogChannels",
                column: "GuildId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogChannels");
        }
    }
}
