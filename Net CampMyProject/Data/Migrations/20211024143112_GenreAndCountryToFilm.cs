using Microsoft.EntityFrameworkCore.Migrations;

namespace Net_CampMyProject.Data.Migrations
{
    public partial class GenreAndCountryToFilm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Films",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Films",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Films");
        }
    }
}
