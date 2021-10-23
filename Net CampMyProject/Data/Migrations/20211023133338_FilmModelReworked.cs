using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Net_CampMyProject.Data.Migrations
{
    public partial class FilmModelReworked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Films_FilmId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Films",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ImbId",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Films",
                newName: "Screenplay");

            migrationBuilder.RenameColumn(
                name: "RankUpDown",
                table: "Films",
                newName: "Nominations");

            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "Films",
                newName: "Languages");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Films",
                newName: "ImgUrl");

            migrationBuilder.RenameColumn(
                name: "ImDbRatingCount",
                table: "Films",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "ImDbRating",
                table: "Films",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "FullTitle",
                table: "Films",
                newName: "Budget");

            migrationBuilder.RenameColumn(
                name: "Crew",
                table: "Films",
                newName: "BoxOffice");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Films",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Awards",
                table: "Films",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Films",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "FilmId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Films",
                table: "Films",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FilmRatingSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmRatingSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthnIformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialNetworksLinks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilmRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmRatings_FilmRatingSources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "FilmRatingSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmRatings_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilmPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmPersons_Films_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Films",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmPersons_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmPersons_FilmId",
                table: "FilmPersons",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmPersons_PersonId",
                table: "FilmPersons",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmRatings_FilmId",
                table: "FilmRatings",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmRatings_SourceId",
                table: "FilmRatings",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Films_FilmId",
                table: "Comments",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Films_FilmId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "FilmPersons");

            migrationBuilder.DropTable(
                name: "FilmRatings");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "FilmRatingSources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Films",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Awards",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "Screenplay",
                table: "Films",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "Nominations",
                table: "Films",
                newName: "RankUpDown");

            migrationBuilder.RenameColumn(
                name: "Languages",
                table: "Films",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "Films",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Films",
                newName: "ImDbRatingCount");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Films",
                newName: "ImDbRating");

            migrationBuilder.RenameColumn(
                name: "Budget",
                table: "Films",
                newName: "FullTitle");

            migrationBuilder.RenameColumn(
                name: "BoxOffice",
                table: "Films",
                newName: "Crew");

            migrationBuilder.AddColumn<string>(
                name: "ImbId",
                table: "Films",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "FilmId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Films",
                table: "Films",
                column: "ImbId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Films_FilmId",
                table: "Comments",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "ImbId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
