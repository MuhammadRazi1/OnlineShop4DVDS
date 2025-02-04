using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineShop4DVDS.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtistRoles",
                columns: table => new
                {
                    ArtistRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtistRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistRoles", x => x.ArtistRoleId);
                });

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    ArtistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtistRoleId = table.Column<int>(type: "int", nullable: false),
                    ArtistName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArtistAge = table.Column<int>(type: "int", nullable: false),
                    ArtistDateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    ArtistBio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtistImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.ArtistId);
                    table.ForeignKey(
                        name: "FK_Artists_ArtistRoles_ArtistRoleId",
                        column: x => x.ArtistRoleId,
                        principalTable: "ArtistRoles",
                        principalColumn: "ArtistRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ArtistRoles",
                columns: new[] { "ArtistRoleId", "ArtistRoleName" },
                values: new object[,]
                {
                    { 1, "Singer" },
                    { 2, "Composer" },
                    { 3, "Both" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artists_ArtistRoleId",
                table: "Artists",
                column: "ArtistRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "ArtistRoles");
        }
    }
}
