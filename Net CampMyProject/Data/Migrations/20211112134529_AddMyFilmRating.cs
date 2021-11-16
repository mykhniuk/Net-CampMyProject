using Microsoft.EntityFrameworkCore.Migrations;

namespace Net_CampMyProject.Data.Migrations
{
    public partial class AddMyFilmRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyFilmRating",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyRating = table.Column<bool>(type: "bit", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyFilmRating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyFilmRating_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MyFilmRating_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyFilmRating_AuthorId",
                table: "MyFilmRating",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_MyFilmRating_FilmId",
                table: "MyFilmRating",
                column: "FilmId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyFilmRating");
        }
    }
}
