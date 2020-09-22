using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Hiromi.Data.Migrations
{
    public partial class AddWelcomeResponses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WelcomeResponses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChannelId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcomeResponses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WelcomeResponses_ChannelId",
                table: "WelcomeResponses",
                column: "ChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WelcomeResponses_GuildId",
                table: "WelcomeResponses",
                column: "GuildId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WelcomeResponses");
        }
    }
}
