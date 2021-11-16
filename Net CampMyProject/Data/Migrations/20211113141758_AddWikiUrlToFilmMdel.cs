using Microsoft.EntityFrameworkCore.Migrations;

namespace Net_CampMyProject.Data.Migrations
{
    public partial class AddWikiUrlToFilmMdel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WikiUrl",
                table: "Films",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WikiUrl",
                table: "Films");
        }
    }
}
