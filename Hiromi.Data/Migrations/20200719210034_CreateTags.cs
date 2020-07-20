using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Hiromi.Data.Migrations
{
    public partial class CreateTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Uses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.UniqueConstraint("AK_Tags_GuildId_Name", x => new { x.GuildId, x.Name });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
